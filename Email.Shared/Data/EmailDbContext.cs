using Microsoft.EntityFrameworkCore;
using Email.Shared.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Email.Shared.Data
{
    public class EmailDbContext : DbContext
	{
		public EmailDbContext(DbContextOptions options) : base(options)
		{
		}

		public DbSet<ServiceClient> Clients { get; set; }
		public DbSet<EmailTrail> Emails { get; set; }
		public DbSet<Failed> Failed { get; set; }
    }
}
