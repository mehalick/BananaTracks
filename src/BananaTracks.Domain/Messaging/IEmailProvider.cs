namespace BananaTracks.Domain.Messaging;

public interface IEmailProvider
{
	Task Send(string email, string subject, string text, string html);
}
