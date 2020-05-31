using Email.Api.Models;
using Email.Api.Models.Enums;
using Email.Api.Models.Requests;
using Email.Shared;
using Email.Shared.DTO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Threading.Tasks;
using static Email.Shared.MessageSerializer;

namespace Email.Api.Services
{
	public interface IRabbitService
	{
		Task<ApiResponse<Guid>> QueueAsync(string clientName, SendEmailRequest request);
	}


	public class RabbitService : IRabbitService
	{
		private static readonly IConnection _connection;
		private static readonly ConnectionFactory _connectionFactory;
		private static readonly RabbitConfig _config;

		private readonly ILogger _logger;

		static RabbitService()
		{
			_config = new RabbitConfig();
			Program.Config.GetSection("RabbitMQ").Bind(_config);
			_connectionFactory = new ConnectionFactory() { HostName = _config.HostName, UserName = _config.Username, Password = _config.Password };
			_connection = _connectionFactory.CreateConnection();
		}

		public RabbitService(ILoggerFactory logger)
		{
			_logger = logger.CreateLogger<RabbitService>();
		}


		public Task<ApiResponse<Guid>> QueueAsync(string clientName, SendEmailRequest request)
		{
			var trailId = Guid.NewGuid();

			try
			{
				var dto = BuildDto(trailId, request);
				var success = SendToQueue(dto);

				if (success)
				{
					_logger.LogInformation("Email added to queue (ID {id})", dto.Id);
				}
				else
				{
					_logger.LogWarning("Failed to add email to queue; client: {client}", clientName);
				}

				return Task.FromResult(new ApiResponse<Guid>(trailId));

			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Exception caught while trying to process a request from client {0}; email trail ID: {1}", clientName, trailId);
				return Task.FromResult(new ApiResponse<Guid>(new Error(ErrorCode.UnhandledException)));
			}
		}


		private EmailDto BuildDto(Guid id, SendEmailRequest request) => new EmailDto
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
