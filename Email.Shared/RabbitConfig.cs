using System;
using System.Collections.Generic;
using System.Text;

namespace Email.Shared
{
    public class RabbitConfig
    {
		public string HostName { get; set; }

		public string QueueName { get; set; }

		public string Username { get; set; }

		public string Password { get; set; }

		public bool Durable { get; set; }
	}
}
