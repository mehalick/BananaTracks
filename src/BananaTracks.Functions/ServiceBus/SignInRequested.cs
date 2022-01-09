namespace BananaTracks.Functions.ServiceBus;

public class SignInRequested
{
	private readonly ICosmosContext _cosmosContext;
	private readonly IEmailProvider _emailProvider;
	private readonly AppSettings _appSettings;
	private readonly UrlSecurity _urlSecurity;

	public SignInRequested(ICosmosContext cosmosContext, IEmailProvider emailProvider, IConfiguration configuration,
		IOptions<AppSettings> appSettings)
	{
		_cosmosContext = cosmosContext;
		_emailProvider = emailProvider;
		_appSettings = appSettings.Value;
		_urlSecurity = new UrlSecurity(configuration);
	}

	[Function(nameof(SignInRequested))]
	public async Task Run([ServiceBusTrigger("signin-requested", Connection = "BananaTracks:Connections:ServiceBus")] SignInRequestedMessage message)
	{
		var tenant = _appSettings.TenantsById[message.TenantId];

		var userId = await _cosmosContext.Users
			.AsNoTracking()
			.WithPartitionKey(tenant.Id.ToString())
			.Where(i => i.Email == message.Email)
			.Select(i => i.Id)
			.SingleOrDefaultAsync();

		string text, html;

		if (userId == Guid.Empty)
		{
			text = html = "We're sorry but an account was not found for this email address.";
		}
		else
		{
			var signInUrl = _urlSecurity.GenerateSecureUrl($"https://{tenant.Host}/auth/sign-in", new Dictionary<string, object>
			{
				["email"] = message.Email
			});

			text = $"Here's your sign-in link: {signInUrl}.";
			html = $"Here's your sign-in link: <a href=\"{signInUrl}\" clicktracking=\"off\">Sign In</a>.";
		}

		await _emailProvider.Send(message.Email, "Sign In Link", text, html);
	}
}
