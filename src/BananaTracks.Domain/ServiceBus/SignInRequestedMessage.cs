namespace BananaTracks.Domain.ServiceBus;

public class SignInRequestedMessage : ServiceBusMessageBase
{
	public string Email { get; set; } = "";

	public SignInRequestedMessage() : base("signin-requested")
	{
	}
}
