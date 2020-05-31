using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Email.Api.Models;
using Email.Api.Models.Enums;
using Email.Shared.Data;
using Email.Shared.Data.Entities;
using System;
using System.Threading.Tasks;

namespace Email.Api.Services
{
	public interface IStatusService
	{
		Task<ApiResponse<string>> GetEmailStatusAsync(string clientName, long trailId);
	}


	public class StatusService : IStatusService
	{
		private readonly ILogger _logger;
		private readonly DbSet<EmailTrail> _emails;

		public StatusService(EmailDbContext dbContext, ILoggerFactory logger)
		{
			_logger = logger.CreateLogger<StatusService>();
			_emails = dbContext.Set<EmailTrail>();
		}


		public async Task<ApiResponse<string>> GetEmailStatusAsync(string clientName, long trailId)
		{
			try
			{
				var trail = await _emails.FindAsync(trailId);

				if (trail == null)
				{
					_logger.LogWarning("Client {0} requested the status of a non-existing email trail with ID {1}", clientName, trailId);
					return new ApiResponse<string>(new Error(ErrorCode.InvalidRequestParameterValue));
				}
				else if (!trail.RequestOrigin.Equals(clientName))
				{
					_logger.LogWarning("Client {0} requested the status of an email trail with ID {1}, which it didn't create", clientName, trailId);
					return new ApiResponse<string>(new Error(ErrorCode.ClientUnauthorized));
				}

				return new ApiResponse<string>(trail.Status.ToString());
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Exception caught while trying to retrieve status of email trail with ID: {id}", trailId);
				return new ApiResponse<string>(new Error(ErrorCode.UnhandledException));
			}
		}
	}
}
