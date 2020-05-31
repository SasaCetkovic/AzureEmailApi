using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;
using System.IO;

namespace Email.Api
{
	public class Program
    {
		private static IConfigurationRoot _config;

		public static IConfiguration Config { get; private set; }

		public static void Main(string[] args)
        {
			_config = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("hosting.json", optional: true)
				.Build();

			var host = BuildWebHost(args);
			Config = host.Services.GetService<IConfiguration>();
			host.Run();
		}

		public static IHost BuildWebHost(string[] args) =>
            Host.CreateDefaultBuilder(args)
			    .UseNLog()
				.ConfigureWebHostDefaults(webBuilder =>
                {
					webBuilder.UseConfiguration(_config);
					webBuilder.ConfigureLogging(ConfigureLogger);
					webBuilder.UseStartup<Startup>();
				})
				.Build();

		private static void ConfigureLogger(WebHostBuilderContext ctx, ILoggingBuilder logging)
		{
			logging.AddConfiguration(ctx.Configuration.GetSection("Logging"));
			logging.AddDebug();
		}
	}
}
