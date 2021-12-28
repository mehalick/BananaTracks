using BananaTracks.Domain;
using BananaTracks.Domain.Abstractions;
using BananaTracks.Domain.Configuration;
using BananaTracks.Domain.Messaging;
using BananaTracks.Domain.Security;
using BananaTracks.Domain.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

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
		var tenant = _appSettings.TenantsById[message.TenantIdId];

		var usersExist = await _cosmosContext.Users.AnyAsync(x => x.TenantId == tenant.Id);

		if (!usersExist)
		{
			var user = new User(tenant.Id, message.Email, "", Claims.Administrator);

			await _cosmosContext.Users.AddAsync(user);
			await _cosmosContext.SaveChangesAsync();
		}

		var signInUrl = _urlSecurity.GenerateSecureUrl($"https://{tenant.Host}/auth/sign-in", new Dictionary<string, object>
		{
			["email"] = message.Email
		});

		var text = $"Here's your sign-in link: {signInUrl}.";
		var html = $"Here's your sign-in link: <a href=\"{signInUrl}\" clicktracking=\"off\">Sign In</a>.";

		await _emailProvider.Send(message.Email, "Sign In Link", text, html);
	}
}
