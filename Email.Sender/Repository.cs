using Microsoft.EntityFrameworkCore;
using Email.Shared.Data;
using Email.Shared.Data.Entities;
using Email.Shared.Data.Enums;
using Email.Shared.DTO;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace Email.Sender
{
	internal static class Repository
	{
		private static string _connectionString;
		private static DbContextOptionsBuilder<EmailDbContext> _optBuilder;


		internal static void Init(string connectionString)
		{
			_connectionString = connectionString;
			_optBuilder = new DbContextOptionsBuilder<EmailDbContext>();
			_optBuilder.UseSqlServer(_connectionString);
		}


		internal static EmailDbContext CreateDbContext()
		{
			return new EmailDbContext(_optBuilder.Options);
		}


		internal static async Task UpdateTrailStatus(long trailId, bool success)
		{
			using (var dbContext = CreateDbContext())
			{
				var trail = await dbContext.Emails.FindAsync(trailId);

				if (trail.Status == EmailStatus.FailedToSend && success)
				{
					var failed = await dbContext.Failed.FindAsync(trailId);
					if (failed != null)
					{
						dbContext.Remove(failed); 
					}
				}

				trail.Status = success ? EmailStatus.Sent : EmailStatus.FailedToSend;
				trail.UpdatedDateTime = DateTime.UtcNow;
				await dbContext.SaveChangesAsync();
			}
		}


		internal static async Task PersistFailedMessage(EmailDto dto, Exception ex)
		{
			using (var dbContext = CreateDbContext())
			{
				var trail = await dbContext.Emails.FindAsync(dto.Id);

				if (trail.Status != EmailStatus.FailedToSend)
				{
					var message = new Failed
					{
						Id = dto.Id,
						Receiver = dto.Receiver,
						Sender = dto.Sender,
						Subject = dto.Subject,
						Body = dto.Body,
						CreatedDateTime = DateTime.UtcNow,
						Error = ex?.ToString()
					};

					await dbContext.AddAsync(message);
					trail.Status = EmailStatus.FailedToSend;
					await dbContext.SaveChangesAsync();
				}
			}
		}


		internal static async Task<bool> EmailSentAsync(long trailId)
		{
			using (var dbContext = CreateDbContext())
			{
				var trail = await dbContext.Emails.FindAsync(trailId);
				return trail.Status == EmailStatus.Sent;
			}
		}


		internal static bool EmailSent(long trailId)
		{
			using (var dbContext = CreateDbContext())
			{
				var trail = dbContext.Emails.Find(trailId);
				return trail.Status == EmailStatus.Sent;
			}
		}


		internal static Failed[] GetFailed()
		{
			using (var dbContext = CreateDbContext())
			{
				var failed = dbContext.Failed.Where(f => f.Trail.Status == EmailStatus.FailedToSend).Include(f => f.Trail).ToArray();
				return failed;
			}
		}
	}
}
