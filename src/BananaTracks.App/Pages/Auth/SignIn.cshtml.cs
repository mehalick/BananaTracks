using System.Security.Claims;
using BananaTracks.Domain;
using BananaTracks.Domain.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace BananaTracks.App.Pages.Auth;

public class SignInModel : PageModel
{
	private readonly Tenant _tenant;
	private readonly UrlSecurity _urlSecurity;
	private readonly ILogger<SignInModel> _logger;

	public SignInModel(ITenantService tenantService, UrlSecurity urlSecurity, ILogger<SignInModel> logger)
	{
		_tenant = tenantService.Tenant;
		_urlSecurity = urlSecurity;
		_logger = logger;
	}

	public IActionResult OnGet(string email, UrlToken urlToken)
	{
		ValidateUrl(email, urlToken);

		return Page();
	}

	public async Task<IActionResult> OnPost(string email, UrlToken urlToken)
	{
		ValidateUrl(email, urlToken);

		// TODO: get use from Cosmos

		await SignIn(email);

		return Redirect("/");
	}

	private void ValidateUrl(string email, UrlToken urlToken)
	{
		var status = _urlSecurity.ValidateSecureUrl(urlToken, email);

		if (status != UrlSecurity.ValidationStatus.Success)
		{
			throw new ArgumentException(status.ToString());
		}
	}

	private async Task SignIn(string email)
	{
		var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
		identity.AddClaim(new Claim(ClaimTypes.Name, email));
		identity.AddClaim(new Claim("tenant_id", _tenant.Id.ToString()));

		await HttpContext.SignInAsync(
			identity.AuthenticationType,
			new ClaimsPrincipal(identity),
			new AuthenticationProperties
			{
				ExpiresUtc = DateTimeOffset.UtcNow.AddYears(1),
				IsPersistent = true
			});

		_logger.LogInformation("Sign-in successful for {Email}", email);
	}
}
