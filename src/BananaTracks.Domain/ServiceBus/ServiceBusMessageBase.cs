namespace BananaTracks.Domain.ServiceBus;

public abstract class ServiceBusMessageBase
{
	public Guid TenantId { get; init; }

	[JsonIgnore]
	public string QueueName { get; }

	protected ServiceBusMessageBase(string queueName)
	{
		QueueName = queueName;
	}
}
