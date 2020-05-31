using System;
using System.Collections.Generic;
using System.Text;

namespace Email.Shared.Data.Enums
{
    public enum EmailStatus
    {
		Unknown,
		RequestReceived,
		InQueue,
		Sent,
		FailedToQueue,
		FailedToSend,
	}
}
