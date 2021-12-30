using System.Security.Claims;
using BananaTracks.Domain;
using Grpc.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace BananaTracks.App.Extensions;

public static class HttpContextExtensions
{
	public static async Task SignInAsync(this HttpContext httpContext, Guid tenantId, User? user)
	{
		if (user is null)
		{
			return;
		}

		var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);

		identity.AddClaim(new Claim("tenant_id", tenantId.ToString()));
		identity.AddClaim(new Claim(ClaimTypes.Name, user.Id.ToString()));
		identity.AddClaim(new Claim(ClaimTypes.GivenName, user.Name));
		identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));

		await httpContext.SignInAsync(
			identity.AuthenticationType,
			new ClaimsPrincipal(identity),
			new AuthenticationProperties
			{
				ExpiresUtc = DateTimeOffset.UtcNow.AddYears(1),
				IsPersistent = true
			});
	}

	public static Guid GetUserId(this ServerCallContext context)
	{
		var httpContext = context.GetHttpContext();
		var user = httpContext.User;

		return Guid.Parse(user.Identity!.Name!);
	}
}
