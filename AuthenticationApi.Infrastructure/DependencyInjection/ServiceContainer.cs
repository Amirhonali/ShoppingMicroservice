using System;
using AuthenticationApi.Application.Interface;
using AuthenticationApi.Infrastructure.Data;
using AuthenticationApi.Infrastructure.Repositories;
using eCommerce.SharedLibrary.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuthenticationApi.Infrastructure.DependencyInjection
{
	public static class ServiceContainer
	{
		public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration configuration)
		{
			//Add database connectivity
			//Jwt Add Authentication Scheme

			SharedServiceContainer.AddSharedService<AuthenticationDbContext>(services, configuration, configuration["MySerilog:FileName"]!);


			//Add Dependency Injection
			services.AddScoped<IUser, UserRepository>();

			return services;
		}

		public static IApplicationBuilder UserInfrastructurePolicy(this IApplicationBuilder app)
		{
			//Register middleware such as:
			//Global Exception : Handle external errors.
			//Listen Only To Api Geteway : block all outsiders call.

			SharedServiceContainer.UseSharedPolicies(app);

			return app;
		}
	}
}

