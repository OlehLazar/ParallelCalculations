using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;

public class RoleAuthorizationMiddleware
{
	private readonly RequestDelegate _next;

	public RoleAuthorizationMiddleware(RequestDelegate next)
	{
		_next = next;
	}

	public async Task InvokeAsync(HttpContext context)
	{
		// Skip auth check for non-protected endpoints
		var endpoint = context.GetEndpoint();
		if (endpoint == null || !endpoint.Metadata.Any(m => m is Microsoft.AspNetCore.Authorization.AuthorizeAttribute))
		{
			await _next(context);
			return;
		}

		if (!context.Request.Headers.TryGetValue("Authorization", out StringValues authHeader) ||
			!authHeader.ToString().StartsWith("Bearer "))
		{
			context.Response.StatusCode = StatusCodes.Status401Unauthorized;
			await context.Response.WriteAsync("Missing or invalid Authorization header.");
			return;
		}

		var token = authHeader.ToString().Substring("Bearer ".Length).Trim();
		var handler = new JwtSecurityTokenHandler();
		JwtSecurityToken jwt;

		try
		{
			jwt = handler.ReadJwtToken(token);
		}
		catch
		{
			context.Response.StatusCode = StatusCodes.Status401Unauthorized;
			await context.Response.WriteAsync("Invalid token.");
			return;
		}

		var claimsIdentity = new ClaimsIdentity(jwt.Claims, "jwt");
		var principal = new ClaimsPrincipal(claimsIdentity);
		context.User = principal;

		var requiredRoles = endpoint.Metadata
			.OfType<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>()
			.Select(attr => attr.Roles)
			.Where(r => !string.IsNullOrWhiteSpace(r))
			.SelectMany(r => r.Split(','))
			.Distinct()
			.ToList();

		if (requiredRoles.Any())
		{
			var userRoles = principal.Claims
			.Where(c => c.Type == ClaimTypes.Role || c.Type == "role")
			.Select(c => c.Value);

			if (!userRoles.Intersect(requiredRoles).Any())
			{
				context.Response.StatusCode = StatusCodes.Status403Forbidden;
				await context.Response.WriteAsync("Access denied. Insufficient role.");
				return;
			}
		}

		await _next(context);
	}
}
