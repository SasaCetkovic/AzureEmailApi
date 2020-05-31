using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Email.Api.Middleware;

namespace Email.Api.Extensions
{
    public static class MiddlewareExtensions
    {
		public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestLoggingMiddleware>();
        }


        public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomExceptionHandlerMiddleware>();
        }


		public static IApplicationBuilder UseSwaggerUiCustomSettings(this IApplicationBuilder builder, IConfiguration configuration)
		{
			builder.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint(configuration.GetValue<string>("Swagger:EndpointUrl"),
								  configuration.GetValue<string>("Swagger:EndpointDescription"));
				c.RoutePrefix = string.Empty;
				c.DisplayRequestDuration();
			});

			return builder;
		}
	}
}
