using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Email.Sender.WinServiceHost
{
	public partial class EmailSenderService : ServiceBase
	{
		private IContainer _components;
		private EventLog _eventLog;

		public EmailSenderService()
		{
			InitializeComponent();

			_eventLog = new EventLog();
			if (!EventLog.SourceExists("EmailSenderSource"))
			{
				EventLog.CreateEventSource(
					"EmailSenderSource", "EmailSenderLog");
			}
			_eventLog.Source = "EmailSenderSource";
			_eventLog.Log = "EmailSenderLog";
		}

		protected override void OnStart(string[] args)
		{
			_eventLog.WriteEntry("Loading configuration...");

			try
			{
				_eventLog.WriteEntry("Service starting...");
				Listener.Start();
				_eventLog.WriteEntry("Service started; awaiting items from queue...");
			}
			catch (Exception ex)
			{
				_eventLog.WriteEntry(ex.ToString(), EventLogEntryType.Error);
			}
		}

		protected override void OnStop()
		{
			_eventLog.WriteEntry("Stopping service...");
		}
	}
}
