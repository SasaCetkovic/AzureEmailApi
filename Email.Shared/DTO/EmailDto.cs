using System;
using System.Collections.Generic;
using System.Text;

namespace Email.Shared.DTO
{
	[Serializable]
    public class EmailDto
    {
		public long Id { get; set; }

		public string Sender { get; set; }

		public string Receiver { get; set; }

		public string Subject { get; set; }

		public string Body { get; set; }
	}
}
