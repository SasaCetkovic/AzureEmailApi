using Email.Shared.Data.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Email.Shared.Data.Entities
{
    public class ServiceClient : Entity
	{
		[StringLength(32)]
		public string Username { get; set; }

		[StringLength(64)]
		public string Password { get; set; }
	}
}
