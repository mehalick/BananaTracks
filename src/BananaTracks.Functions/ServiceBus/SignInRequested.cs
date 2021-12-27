using BananaTracks.Domain.Configuration;
using BananaTracks.Domain.Messaging;
using BananaTracks.Domain.Security;
using BananaTracks.Domain.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace BananaTracks.Functions.ServiceBus;

public class SignInRequested
{
	private readonly IEmailProvider _emailProvider;
	private readonly AppSettings _appSettings;
	private readonly UrlSecurity _urlSecurity;

	public SignInRequested(IConfiguration configuration, IOptions<AppSettings> appSettings, IEmailProvider emailProvider)
	{
		_appSettings = appSettings.Value;
		_urlSecurity = new UrlSecurity(configuration);
		_emailProvider = emailProvider;
	}

	[Function(nameof(SignInRequested))]
	public async Task Run([ServiceBusTrigger("signin-requested", Connection = "BananaTracks:Connections:ServiceBus")] SignInRequestedMessage message)
	{
		var tenant = _appSettings.TenantsById[message.TenantId];

		var signInUrl = _urlSecurity.GenerateSecureUrl($"https://{tenant.Host}/auth/signin", new Dictionary<string, object>
		{
			["email"] = message.Email
		});

		var text = $"Here's your sign-in link: {signInUrl}.";
		var html = $"Here's your sign-in link: <a href=\"{signInUrl}\" clicktracking=\"off\">Sign In</a>.";

		await _emailProvider.Send(message.Email, "Sign In Link", text, html);
	}
}
