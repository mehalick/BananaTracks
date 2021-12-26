namespace BananaTracks.Domain.ServiceBus;

public interface IServiceBusProvider
{
	Task Send(ServiceBusMessageBase message, CancellationToken cancellationToken);
}
