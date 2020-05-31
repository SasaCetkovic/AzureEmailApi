using Bazinga.AspNetCore.Authentication.Basic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Email.Shared.Data;
using Email.Shared.Data.Entities;
using System;
using System.Threading.Tasks;

namespace Email.Api.Services
{
	public class AuthService : IBasicCredentialVerifier
	{
		private readonly ILogger _logger;
		private readonly DbSet<ServiceClient> _clients;

		public AuthService(EmailDbContext context, ILoggerFactory logger)
		{
			_logger = logger.CreateLogger<AuthService>();
			_clients = context.Set<ServiceClient>();
		}

		public async Task<bool> Authenticate(string username, string password)
		{
			try
			{
				var client = await _clients.SingleAsync(c => c.Username == username);
				return client.Password == password;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error trying to authenticate a client - username: {0}; password: {1}", username, password);
				return false;
			}
		}
	}
}
