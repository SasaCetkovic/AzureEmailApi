using Email.Shared.Data.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Email.Shared.Data.Entities
{
    public class Failed : Entity
	{
		public Failed()
		{
			CreatedDateTime = DateTime.UtcNow;
		}


		[ForeignKey("Id")]
		public EmailTrail Trail { get; set; }

		[StringLength(255)]
		public string Sender { get; set; }

		[StringLength(255)]
		public string Receiver { get; set; }

		[StringLength(255)]
		public string Subject { get; set; }

		public string Body { get; set; }

		public string Error { get; set; }

		public DateTime CreatedDateTime { get; set; }
	}
}
