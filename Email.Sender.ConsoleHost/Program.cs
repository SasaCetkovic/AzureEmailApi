using System;
using System.Threading;

namespace Email.Sender.ConsoleHost
{
	class Program
    {
        static void Main()
        {
            Console.WriteLine("Loading...");

			Listener.Start();

			Console.WriteLine("Service started; awaiting items from queue...");
			new AutoResetEvent(false).WaitOne();
		}
	}
}
