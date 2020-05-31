using Email.Api.Models.Enums;
using System;
using System.Collections.Generic;

namespace Email.Api.Models
{
	public static class ErrorMessages
	{
		private static readonly Dictionary<ErrorCode, string> _errors = new Dictionary<ErrorCode, string>
		{
			[ErrorCode.UnknownError]                 = null,
			[ErrorCode.UnhandledException]           = "The service encountered an unhandled exception",
			[ErrorCode.ConfigurationError]           = "Error in service configuration",
			[ErrorCode.ClientUnauthorized]           = "The client is not authorized to perform this operation",
			[ErrorCode.UserNotFound]                 = "The requested user cannot be found",
			[ErrorCode.InvalidRequestParameterValue] = "Invalid parameter value was provided in the request",
		};

		public static string Get(ErrorCode code) => _errors[code];
	}
}
