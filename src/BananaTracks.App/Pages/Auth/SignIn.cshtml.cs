using BananaTracks.Domain.Entities;

namespace BananaTracks.App.Pages.Auth;

public class SignInModel : PageModel
{
	private readonly Tenant _tenant;
	private readonly ICosmosContext _cosmosContext;
	private readonly UrlSecurity _urlSecurity;
	private readonly ILogger<SignInModel> _logger;

	public SignInModel(ITenantService tenantService, ICosmosContext cosmosContext, UrlSecurity urlSecurity, ILogger<SignInModel> logger)
	{
		_tenant = tenantService.Tenant;
		_cosmosContext = cosmosContext;
		_urlSecurity = urlSecurity;
		_logger = logger;
	}

	public IActionResult OnGet(string email, UrlToken urlToken)
	{
		ValidateUrl(email, urlToken);

		return Page();
	}

	public async Task<IActionResult> OnPost(string email, UrlToken urlToken, CancellationToken cancellationToken)
	{
		ValidateUrl(email, urlToken);

		// TODO: get use from Cosmos

		await SignIn(email, cancellationToken);

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

	private async Task SignIn(string email, CancellationToken cancellationToken)
	{
		var user = await _cosmosContext.Users
			.AsNoTracking()
			.WithPartitionKey(_tenant.Id.ToString())
			.Where(i => i.Email == email)
			.SingleOrThrowAsync(cancellationToken);

		await HttpContext.SignInAsync(_tenant.Id, user);

		_logger.LogInformation("Sign-in successful for {Email}", email);
	}
}
