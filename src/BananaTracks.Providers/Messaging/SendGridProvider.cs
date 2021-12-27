using BananaTracks.Domain.Messaging;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace BananaTracks.Providers.Messaging;

public class SendGridProvider : IEmailProvider
{
	private readonly SendGridClient _client;
	private readonly EmailAddress _emailSender;

	public SendGridProvider(IConfiguration configuration)
	{
		_client = new SendGridClient(configuration["BananaTracks:SendGrid:ApiKey"]);
		_emailSender = new EmailAddress(configuration["BananaTracks:SendGrid:Sender"], "BananaTracks");
	}

	public async Task Send(string email, string subject, string text, string html)
	{
		var to = new EmailAddress(email);
		var message = MailHelper.CreateSingleEmail(_emailSender, to, subject, text, html);

		await _client.SendEmailAsync(message);
	}
}
