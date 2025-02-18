using System;
using eCommerce.SharedLibrary.Logs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderApi.Application.Interfaces;
using OrderApi.Application.Services;
using Polly;
using Polly.Retry;

namespace OrderApi.Application.DependencyInjection
{
	public static class ServiceContainer
	{
		public static IServiceCollection AddApplicationService(this IServiceCollection services, IConfiguration config)
		{
			//Register HttpClient service
			//Add Dependency Injection
			services.AddHttpClient<IOrderService, OrderService>(option =>
			{
				option.BaseAddress = new Uri(config["ApiGeteway:BaseAddress"]!);
				option.Timeout = TimeSpan.FromSeconds(1);
			});

			//Add Retry Strategy
			var retryStrategy = new RetryStrategyOptions()
			{
				ShouldHandle = new PredicateBuilder().Handle<TaskCanceledException>(),
				BackoffType = DelayBackoffType.Constant,
				UseJitter = true,
				MaxRetryAttempts = 3,
				Delay = TimeSpan.FromMilliseconds(500),
				OnRetry = args =>
				{
					string message = $"OnRetry, Attempt: {args.AttemptNumber} Outcome {args.Outcome}";
					LogException.LogToConsole(message);
					LogException.LogToDebbuger(message);
					return ValueTask.CompletedTask;
				}
			};

			//Use Retry strategy
			services.AddResiliencePipeline("my-retry-pipeline", builder =>
			{
				builder.AddRetry(retryStrategy);
			});


			return services;
		}
	}
}

