using Email.Shared;
using Email.Shared.DTO;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using static Email.Shared.MessageSerializer;

namespace Email.Sender
{
	public static class Listener
	{
		private static RabbitConfig _rmqConfig;
		private static ConnectionFactory _connectionFactory;
		private static IConnection _connection;
		private static IModel _channel;
		private static EventingBasicConsumer _consumer;

#pragma warning disable CA1810 // Initialize reference type static fields inline
		static Listener()
#pragma warning restore CA1810 // Initialize reference type static fields inline
		{
			var config = new ConfigurationBuilder()
				.AddJsonFile("settings.json", true, true)
				.Build();


			_rmqConfig = new RabbitConfig();
			config.GetSection("RabbitMQ").Bind(_rmqConfig);

			var emailSettings = new EmailSettings();
			config.GetSection("SmtpServer").Bind(emailSettings);

			EmailSender.Init(emailSettings);
			Repository.Init(config.GetConnectionString("DefaultConnection"));
		}


		public static void Start()
		{
			Connect();

			_consumer = new EventingBasicConsumer(_channel);
			_consumer.Received += ParseMessage;

			_channel.BasicConsume(queue: _rmqConfig.QueueName,
								 autoAck: false,
								 consumer: _consumer);
		}


		public static void TryResendFailed()
		{
			var failed = Repository.GetFailed();
			foreach (var f in failed)
			{
				if (!Repository.EmailSent(f.Id))
				{
					var dto = new EmailDto { Id = f.Id, Body = f.Body, Receiver = f.Receiver, Sender = f.Sender, Subject = f.Subject };

					try
					{
						var success = EmailSender.SendAsync(dto).Result;
						if (success)
						{
							Repository.UpdateTrailStatus(dto.Id, success).Wait();
						}
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex);
					}
				}
			}
		}


		private static void Connect()
		{
			_connectionFactory = new ConnectionFactory() { HostName = _rmqConfig.HostName, UserName = _rmqConfig.Username, Password = _rmqConfig.Password };
			_connection = _connectionFactory.CreateConnection();
			_channel = _connection.CreateModel();
			_channel.QueueDeclare(_rmqConfig.QueueName, _rmqConfig.Durable, false, false);
			_channel.BasicQos(0, 1, false);
		}


		private static async void ParseMessage(object model, BasicDeliverEventArgs ea)
		{
#if DEBUG
			Console.WriteLine(" [x] Received message - delivery tag: {0}", ea.DeliveryTag);
#endif

			var success = false;

			try
			{
				var message = DeserialiseFromBinary<EmailDto>(ea.Body);

				if (!await Repository.EmailSentAsync(message.Id))
				{
					success = await EmailSender.SendAsync(message);
				}

				if (success)
				{
					_channel.BasicAck(ea.DeliveryTag, false);
					await Repository.UpdateTrailStatus(message.Id, success); 
				}
				else
				{
					// status already set to FailedToSend
					_channel.BasicNack(ea.DeliveryTag, false, true);
				}
			}
			catch (Exception ex)
			{
				// cannot allow the method to fail
#if DEBUG
				Console.WriteLine(" [x] Failed - {0}", ex.Message);
#endif
			}

#if DEBUG
			Console.WriteLine(" [x] Done");
#endif
		}
	}
}
