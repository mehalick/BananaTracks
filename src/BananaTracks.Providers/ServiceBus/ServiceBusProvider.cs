using System.Text.Json;
using Azure.Messaging.ServiceBus;

namespace BananaTracks.Providers.ServiceBus;

public class ServiceBusProvider : IServiceBusProvider
{
	private readonly ServiceBusClient _client;

	public ServiceBusProvider(ServiceBusClient client)
	{
		_client = client;
	}

	public async Task Send(ServiceBusMessageBase message, CancellationToken cancellationToken)
	{
		var sender = _client.CreateSender(message.QueueName);
		var json = JsonSerializer.Serialize(message, message.GetType());

		await sender.SendMessageAsync(new ServiceBusMessage(json), cancellationToken);
	}
}
