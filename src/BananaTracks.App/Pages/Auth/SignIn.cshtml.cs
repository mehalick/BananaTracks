using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using BananaTracks.Domain.ServiceBus;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace BananaTracks.App.Pages.Auth;

public class SignInModel : PageModel
{
	[BindProperty]
	[Required, EmailAddress]
	public string Email { get; set; } = "andy.mehalick@outlook.com";

	private readonly ITenantService _tenantService;
	private readonly IServiceBusProvider _serviceBusProvider;
	private readonly ILogger<SignInModel> _logger;

	public SignInModel(ITenantService tenantService, IServiceBusProvider serviceBusProvider, ILogger<SignInModel> logger)
	{
		_tenantService = tenantService;
		_serviceBusProvider = serviceBusProvider;
		_logger = logger;
	}

	public async Task<IActionResult> OnGet(string? email)
	{
		if (string.IsNullOrWhiteSpace(email))
		{
			return Page();
		}

		await SignIn(email);

		return Redirect("/");
	}

	private async Task SignIn(string email)
	{
		var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
		identity.AddClaim(new Claim(ClaimTypes.Name, email));
		identity.AddClaim(new Claim("tenant_id", _tenantService.Tenant.Id.ToString()));

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

	public async Task<IActionResult> OnPost(CancellationToken cancellationToken)
	{
		if (!ModelState.IsValid)
		{
			return Page();
		}

		await _serviceBusProvider.Send(new SignInRequestedMessage
		{
			TenantId = _tenantService.Tenant.Id,
			Email = Email
		}, cancellationToken);

		_logger.LogInformation("Sign-in request sent to service bus for {Email}", Email);

		return Page();
	}
}
