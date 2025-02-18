using System;
using eCommerce.SharedLibrary.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderApi.Application.Interfaces;
using OrderApi.Infrastructure.Data;
using OrderApi.Infrastructure.Repositories;

namespace OrderApi.Infrastructure.DependencyInjection
{
	public static class ServiceContainer
	{
		public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration config)
		{
			//Add Database Connection
			//Add authentication scheme
			SharedServiceContainer.AddSharedService<OrderDbContext>(services, config, config["MySerilog:FileName"]!);

			//Add Dependency Injection
			services.AddScoped<IOrder, OrderRepository>();

			return services;
		}

		public static IApplicationBuilder UserInfrastructurePolicy(this IApplicationBuilder app)
		{
			//Register middleware such as:
			//Global Exception -> handle external errors
			//ListenToApiGateway Only -> block all outsiders calls
			SharedServiceContainer.UseSharedPolicies(app);

			return app;
		}
	}
}

