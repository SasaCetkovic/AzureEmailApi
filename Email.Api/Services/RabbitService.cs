using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Email.Api.Models;
using Email.Api.Models.Enums;
using Email.Api.Models.Requests;
using Email.Shared;
using Email.Shared.Data;
using Email.Shared.Data.Entities;
using Email.Shared.Data.Enums;
using Email.Shared.DTO;
using System;
using System.Threading.Tasks;
using static Email.Shared.MessageSerializer;

namespace Email.Api.Services
{
	public interface IRabbitService
	{
		Task<ApiResponse<long>> QueueAsync(string clientName, SendEmailRequest request);
	}


	public class RabbitService : IRabbitService
	{
		private static readonly IConnection _connection;
		private static readonly ConnectionFactory _connectionFactory;
		private static readonly RabbitConfig _config;

		private readonly ILogger _logger;
		private readonly EmailDbContext _dbContext;
		private readonly DbSet<EmailTrail> _emails;

		static RabbitService()
		{
			_config = new RabbitConfig();
			Program.Config.GetSection("RabbitMQ").Bind(_config);
			_connectionFactory = new ConnectionFactory() { HostName = _config.HostName, UserName = _config.Username, Password = _config.Password };
			_connection = _connectionFactory.CreateConnection();
		}

		public RabbitService(EmailDbContext dbContext, ILoggerFactory logger)
		{
			_logger = logger.CreateLogger<RabbitService>();
			_emails = dbContext.Set<EmailTrail>();
			_dbContext = dbContext;
		}


		public async Task<ApiResponse<long>> QueueAsync(string clientName, SendEmailRequest request)
		{
			var trail = new EmailTrail
			{
				RequestOrigin = clientName,
				Status = EmailStatus.RequestReceived
			};

			try
			{
				await _emails.AddAsync(trail);
				await _dbContext.SaveChangesAsync();

				var dto = BuildDto(trail.Id, request);
				var success = SendToQueue(dto);

				if (success)
				{
					_logger.LogInformation("Email added to queue (ID {id})", dto.Id);
					trail.Status = EmailStatus.InQueue;
					trail.UpdatedDateTime = DateTime.UtcNow;
				}
				else
				{
					_logger.LogWarning("Failed to add email to queue; client: {client}", clientName);
					_logger.LogInformation("Saving email data to database (ID {id})...", dto.Id);
					trail.Status = EmailStatus.FailedToQueue;
					trail.UpdatedDateTime = DateTime.UtcNow;
					trail.Failed = new Failed
					{
						Receiver = dto.Receiver,
						Sender = dto.Sender,
						Subject = dto.Subject,
						Body = dto.Body,
						CreatedDateTime = DateTime.UtcNow,
					};
				}

				_emails.Update(trail);
				await _dbContext.SaveChangesAsync();

				return new ApiResponse<long>(trail.Id);

			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Exception caught while trying to process a request from client {0}; email trail ID: {1}", clientName, trail.Id);
				return new ApiResponse<long>(new Error(ErrorCode.UnhandledException));
			}
		}


		private EmailDto BuildDto(long id, SendEmailRequest request) => new EmailDto
		{
			Id = id,
			Sender = request.Sender,
			Receiver = request.Receiver,
			Subject = request.Subject,
			Body = request.Body
		};


		private bool SendToQueue(EmailDto dto)
		{
			try
			{
				using (var channel = _connection.CreateModel())
				{
					channel.QueueDeclare(_config.QueueName, _config.Durable, false, false);

					var body = SerializeIntoBinary(dto);
					var properties = channel.CreateBasicProperties();
					properties.Persistent = true;

					channel.BasicPublish(string.Empty, _config.QueueName, properties, body);
				}

				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Exception caught while trying to send a message to RabbitMQ; Email trail ID {id}", dto.Id);
				return false;
			}
		}
	}
}
