using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Email.Api.Services;
using System.IO;
using Microsoft.OpenApi.Models;

namespace Email.Api.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection ConfigureSwagger(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo { Title = configuration.GetValue<string>("Swagger:Title"), Version = "v1" });
				var basePath = PlatformServices.Default.Application.ApplicationBasePath;
				var xmlPath = Path.Combine(basePath, configuration.GetValue<string>("Swagger:XmlDocFileName"));
				c.IncludeXmlComments(xmlPath);
			});

			return services;
		}

		public static IServiceCollection ConfigureCustomDependencies(this IServiceCollection services)
		{
			services.AddScoped<IRabbitService, RabbitService>();

			return services;
		}
	}
}
