using System;
using System.Net;
using System.Text.Json;
using eCommerce.SharedLibrary.Logs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce.SharedLibrary.Middleware
{
	public class GlobalException
	{
        private readonly RequestDelegate _next;

        public GlobalException(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
		{
			//Declare default varaibles 
			string message = "sorry, internal server error occured. Kindly try again!!!";
			int statusCode = (int)HttpStatusCode.InternalServerError;
			string title = "Error";

			try
			{
				await _next(context);

				//check if Response is Too Many Request // 429 status code.

				if(context.Response.StatusCode == StatusCodes.Status429TooManyRequests)
				{
					title = "Warning";
					message = "Too many request made.";
					statusCode = StatusCodes.Status429TooManyRequests;
					await ModifyHeader(context, title, message, statusCode);
				}

                //if Response is UnAuthorizedt // 401 status code.
				if(context.Response.StatusCode == StatusCodes.Status401Unauthorized)
				{
					title = "Alert";
					message = "You are not authorized to access!!!";
					await ModifyHeader(context, title, message, statusCode);
				}

				//if Response is Forbidden // 403 status code
				if(context.Response.StatusCode == StatusCodes.Status403Forbidden)
				{
					title = "Out of Access";
					message = "You are not allowed/required to access.";
					statusCode = StatusCodes.Status403Forbidden;
					await ModifyHeader(context, title, message, statusCode);
				}

            }
			catch(Exception ex)
			{
				// Log Original Exceptions / Console
				LogException.LogExceptions(ex);

				//check if Exception is Timout // 408 required timeout
				if(ex is TaskCanceledException || ex is TimeoutException)
				{
					title = "Out of time";
					message = "Request timeout... try again";
					statusCode = StatusCodes.Status408RequestTimeout;
				}

                //if Exception is caught
                //if none of the exception then do the default 
                await ModifyHeader(context, title, message, statusCode);
			}

        }

        private static async Task ModifyHeader(HttpContext context, string title, string message, int statusCode)
        {
			context.Response.ContentType = "application/json";
			await context.Response.WriteAsync(JsonSerializer.Serialize(new ProblemDetails()
			{
				Detail = message,
				Status = statusCode,
				Title = title
			}), CancellationToken.None);
			return;
        }
    }
}

