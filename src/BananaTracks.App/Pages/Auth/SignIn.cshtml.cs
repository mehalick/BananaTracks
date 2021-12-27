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

	public async Task<IActionResult> OnGet(string? id)
	{
		//var autoGenerateSignInId = _environment.IsDevelopment() && id == null && !string.IsNullOrWhiteSpace(Email);

		//if (autoGenerateSignInId )
		//{
		//	using var sha256 = SHA256.Create();
		//	var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(Email));
		//	id = Convert.ToBase64String(hash);
		//}

		if (!string.IsNullOrWhiteSpace(id))
		{
			var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
			identity.AddClaim(new Claim(ClaimTypes.Name, id));
			identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, id));

			await HttpContext.SignInAsync(
				CookieAuthenticationDefaults.AuthenticationScheme,
				new ClaimsPrincipal(identity),
				new AuthenticationProperties
				{
					ExpiresUtc = DateTimeOffset.UtcNow.AddYears(1),
					IsPersistent = true
				});

			return Redirect("/");
		}

		return Page();
	}

	public async Task<IActionResult> OnPost(CancellationToken cancellationToken)
	{
		//if (!ModelState.IsValid)
		//{
		//	return Page();
		//}

		await _serviceBusProvider.Send(new SignInRequestedMessage
		{
			TenantId = _tenantService.Tenant.Id,
			Email = Email
		}, cancellationToken);

		_logger.LogInformation("Great success!");

		return Page();
	}
}
