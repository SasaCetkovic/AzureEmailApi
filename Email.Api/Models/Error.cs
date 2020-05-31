using Email.Api.Models.Enums;
using System;
using System.Collections.Generic;

namespace Email.Api.Models
{
	/// <summary>
	/// Recommended error message format for REST APIs
	/// </summary>
	/// <remarks>
	/// Modelled after Microsoft REST API Guidelines
	/// https://github.com/Microsoft/api-guidelines/blob/vNext/Guidelines.md#710-response-formats
	/// </remarks>
	public class Error
    {
		#region Constructors

		/// <summary>
		/// Creates an <see cref="Error"/> instance
		/// </summary>
		/// <param name="errorCode">Error code</param>
		/// <param name="target">The target of the error (e.g. the name of the property or method in error)</param>
		public Error(ErrorCode errorCode, string target = null)
		{
			Code = errorCode.ToString();
			Message = ErrorMessages.Get(errorCode);
			Target = target;
		}

		/// <summary>
		/// Creates an <see cref="Error"/> instance with a custom message
		/// </summary>
		/// <param name="message">Error message</param>
		public Error(string message)
		{
			Code = ErrorCode.UnknownError.ToString();
			Message = message;
		}

		/// <summary>
		/// Creates an <see cref="Error"/> instance with exception messages in <see cref="Details"/>
		/// </summary>
		/// <param name="errorCode">Error code</param>
		/// <param name="ex">Exception instrance</param>
		/// <param name="target">The target of the error (e.g. the name of the property or method in error)</param>
		public Error(ErrorCode errorCode, Exception ex, string target = null) : this(errorCode, target)
		{
			Details = new List<Error>();

			while (ex != null)
			{
				Details.Add(new Error(ex));
				ex = ex.InnerException;
			}
		}

		private Error(Exception ex)
		{
			Code = ErrorCode.UnhandledException.ToString();
			Message = ex.Message;
		}

		#endregion


		/// <summary>
		/// One of a server-defined set of error codes.
		/// </summary>
        public string Code { get; set; }

		/// <summary>
		/// A human-readable representation of the error.
		/// </summary>
	    public string Message { get; set; }

		/// <summary>
		/// The target of the error (e.g. the name of the property or method in error).
		/// </summary>
	    public string Target { get; set; }

		/// <summary>
		/// An array of details about specific errors that led to this reported error.
		/// </summary>
	    public List<Error> Details { get; set; }
    }
}
