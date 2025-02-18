using System;
using eCommerce.SharedLibrary.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace eCommerce.SharedLibrary.DependencyInjection
{
	public static class SharedServiceContainer
	{
		public static IServiceCollection AddSharedService<TContext>
			(this IServiceCollection services, IConfiguration config, string fileName) where TContext : DbContext
		{
			//Add Generic Database context
			services.AddDbContext<TContext>(option => option.UseNpgsql(config.GetConnectionString("eCommerceConnection")));

			// confiugre serilog logging
			Log.Logger = new LoggerConfiguration()
				.MinimumLevel.Information()
				.WriteTo.Debug()
				.WriteTo.Console()
				.WriteTo.File(path: $"{fileName}-.text",
				restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
				outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {message:lj}{NewLine}{Exception}",
				rollingInterval: RollingInterval.Day)
				.CreateLogger();

			//Add JWT authentication Scheme
			JWTAuthenticationScheme.AddJWTAuthenticationScheme(services, config);
			return services;
		}

		public static IApplicationBuilder UseSharedPolicies(this IApplicationBuilder app)
		{
			//Use global Exception
			app.UseMiddleware<GlobalException>();

			//Register middleware to block all outsiders API calls
			app.UseMiddleware<ListenToOnlyApiGeteway>();

			return app;
		}
	}
}

