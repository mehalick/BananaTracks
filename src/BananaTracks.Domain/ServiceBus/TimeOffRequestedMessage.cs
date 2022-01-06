using BananaTracks.Domain.Configuration;

namespace BananaTracks.Domain.ServiceBus;

public class TimeOffRequestedMessage : ServiceBusMessageBase
{
	public Guid UserId { get; init; }

	[JsonConverter(typeof(DateOnlyJsonConverter))]
	public DateOnly Date { get; init; }

	public TimeOffRequestedMessage() : base("time-off-requested")
	{
	}
}
