using System;
namespace ApiGeteway.Presentation.Middware
{
	public class AttachSignatureToRequest
	{
		private readonly RequestDelegate _next;

		public AttachSignatureToRequest(RequestDelegate next)
		{
			_next = next;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			context.Request.Headers["Api-Geteway"] = "Signed";
			await _next(context);
		}
	}
}

