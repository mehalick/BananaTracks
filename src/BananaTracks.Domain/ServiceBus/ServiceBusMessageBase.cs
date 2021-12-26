using System.Text.Json.Serialization;

namespace BananaTracks.Domain.ServiceBus;

public abstract class ServiceBusMessageBase
{
	public Guid TenantId { get; set; }

	[JsonIgnore]
	public string QueueName { get; }

	protected ServiceBusMessageBase(string queueName)
	{
		QueueName = queueName;
	}
}