using System.Text.Json.Serialization;

namespace BananaTracks.Domain.ServiceBus;

public abstract class ServiceBusMessageBase
{
	public Guid TenantIdId { get; set; }

	[JsonIgnore]
	public string QueueName { get; }

	protected ServiceBusMessageBase(string queueName)
	{
		QueueName = queueName;
	}

	protected ServiceBusMessageBase(string queueName, Guid tenantId)
	{
		QueueName = queueName;
		TenantIdId = tenantId;
	}
}
