using Bazinga.AspNetCore.Authentication.Basic;
using Email.Api.Extensions;
using Email.Api.Services;
using Email.Shared.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

namespace Email.Api
{
	public class Startup
    {
		private readonly IConfiguration _configuration;

		public Startup(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
        {
			var builder = services.AddMvcCore();
			builder.AddAuthorization();
			builder.AddApiExplorer();
			builder.AddDataAnnotations();
			builder.AddFormatterMappings();
			builder.AddNewtonsoftJson(
				options =>
				{
					options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
					options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
					options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
					options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
				});

			services.AddCors(SetupCors);
			services.ConfigureSwagger(_configuration);

			// Basic auth
			services.AddAuthentication(BasicAuthenticationDefaults.AuthenticationScheme)
					.AddBasicAuthentication<AuthService>();

			services.AddDbContext<EmailDbContext>(opt => opt.UseSqlServer(_configuration.GetConnectionString("DefaultConnection")));

			services.ConfigureCustomDependencies();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IServiceProvider serviceProvider, IWebHostEnvironment env)
        {
			if (env.IsDevelopment())
			{
				var dbContext = serviceProvider.GetService<EmailDbContext>();
				dbContext.Database.EnsureCreated();
			}

			app.UseCustomExceptionHandler();
			app.UseSwagger();
			app.UseSwaggerUiCustomSettings(_configuration);

            app.UseRouting();
			app.UseCors("AllowAll");
			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}

		private void SetupCors(CorsOptions options)
		{
			options.AddPolicy("AllowAll", policyBuilder =>
			{
				policyBuilder.AllowAnyOrigin()
							 .AllowAnyMethod()
							 .AllowAnyHeader();
			});
		}
	}
}
