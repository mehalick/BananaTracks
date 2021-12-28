using System.ComponentModel.DataAnnotations;
using BananaTracks.Domain;
using BananaTracks.Domain.Security;
using BananaTracks.Domain.ServiceBus;

namespace BananaTracks.App.Pages.Auth;

public class SendLinkModel : PageModel
{
	[BindProperty]
	[Required, EmailAddress]
	public string Email { get; set; } = "andy.mehalick@outlook.com";

	public Uri? SignInUrl { get; }

	private readonly Tenant _tenant;
	private readonly IServiceBusProvider _serviceBusProvider;
	private readonly ILogger<SendLinkModel> _logger;

	public SendLinkModel(ITenantService tenantService, IServiceBusProvider serviceBusProvider, ILogger<SendLinkModel> logger, IWebHostEnvironment environment, UrlSecurity urlSecurity)
	{
		_tenant = tenantService.Tenant;
		_serviceBusProvider = serviceBusProvider;
		_logger = logger;

		if (environment.IsDevelopment() && !string.IsNullOrWhiteSpace(Email))
		{
			SignInUrl = urlSecurity.GenerateSecureUrl($"https://{_tenant.Host}/auth/sign-in", new Dictionary<string, object>
			{
				["email"] = Email
			});
		}
	}

	public async Task<IActionResult> OnPost(CancellationToken cancellationToken)
	{
		if (!ModelState.IsValid)
		{
			return Page();
		}

		await _serviceBusProvider.Send(new SignInRequestedMessage
		{
			TenantIdId = _tenant.Id,
			Email = Email
		}, cancellationToken);

		_logger.LogInformation("Sign-in request sent to service bus for {Email}", Email);

		return Page();
	}
}
