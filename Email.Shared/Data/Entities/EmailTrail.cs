using Email.Shared.Data.Entities.Base;
using Email.Shared.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Email.Shared.Data.Entities
{
    public class EmailTrail : Entity
	{
		public EmailTrail()
		{
			var now = DateTime.UtcNow;
			CreatedDateTime = now;
			UpdatedDateTime = now;
		}


		[StringLength(32)]
		public string RequestOrigin { get; set; }

		public EmailStatus Status { get; set; }

		public DateTime CreatedDateTime { get; set; }

		public DateTime UpdatedDateTime { get; set; }

		[InverseProperty("Trail")]
		public Failed Failed { get; set; }
	}
}
