using System;
using Microsoft.AspNetCore.Http;

namespace eCommerce.SharedLibrary.Middleware
{
	public class ListenToOnlyApiGeteway
	{

		private readonly RequestDelegate _next;

		public ListenToOnlyApiGeteway(RequestDelegate next)
		{
			_next = next;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			//Extract specific header from the request
			var signedHeader = context.Request.Headers["Api-Geteway"];

			//Null means, the request is not coming from the Api Geteway // 503 service unavaible
			if(signedHeader.FirstOrDefault() is null)
			{
				context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
				await context.Response.WriteAsync("Sorry, service is unavaible");
				return;
			}
			else
			{
				await _next(context);
			}
		}
	}
}

