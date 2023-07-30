using BasicAuthorization.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using System.Net.Http.Headers;
using System.Text;

namespace BasicAuthorization.Middleware;

public class BasicAuthMiddleware
{
    private readonly RequestDelegate _next;
	public BasicAuthMiddleware(RequestDelegate next)
	{
		_next = next;
	}
	public async Task InvokeAsync(HttpContext httpContext, IAccountService accountService)
	{
		try
		{
			var authHeader = AuthenticationHeaderValue.Parse(httpContext.Request.Headers.Authorization);
			var credential = Encoding.UTF8.GetString(Convert.FromBase64String(authHeader.Parameter)).Split(":",2);
			httpContext.Items["user"] = await accountService.AuthenticateAsync(credential[0], credential[1]);

        }
		catch 
		{
			AuthenticateResult.Fail("invalid Authorization header");
		}
		await _next(httpContext);
	}
}
