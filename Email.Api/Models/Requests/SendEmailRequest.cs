using System.ComponentModel.DataAnnotations;

namespace Email.Api.Models.Requests
{
	public class SendEmailRequest
    {
		/// <summary>
		/// Sender's email address
		/// </summary>
		[Required]
		[EmailAddress]
		public string Sender { get; set; }

		/// <summary>
		/// Receiving email address
		/// </summary>
		[Required]
		[EmailAddress]
		public string Receiver { get; set; }

		/// <summary>
		/// Email subject
		/// </summary>
		[Required]
		[StringLength(255)]
		public string Subject { get; set; }

		/// <summary>
		/// Email body
		/// </summary>
		[Required]
		public string Body { get; set; }
	}
}

