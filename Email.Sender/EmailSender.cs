using Email.Shared.DTO;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Email.Sender
{
	internal static class EmailSender
	{
		private static EmailSettings _emailSettings;

		internal static void Init(EmailSettings emailSettings)
		{
			_emailSettings = emailSettings;
		}

		internal static async Task<bool> SendAsync(EmailDto request)
		{
			using (var smtp = new SmtpClient(_emailSettings.Host, _emailSettings.Port))
			{
                smtp.Timeout = 5000;
				smtp.Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password);
				smtp.EnableSsl = _emailSettings.EnableSsl;

                try
				{
					var mailMessage = new MailMessage(request.Sender, request.Receiver, request.Subject, request.Body)
					{
						IsBodyHtml = true
					};

					await smtp.SendMailAsync(mailMessage);
				}
				////catch (SmtpException ex)
				////{
				////	await Repository.PersistFailedMessage(request, ex);
				////	return false;
				////}
				catch (Exception ex)
				{
					await Repository.PersistFailedMessage(request, ex);
					return false;
                }

                return true;
            }
		}
	}
}
