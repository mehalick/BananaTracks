namespace BananaTracks.Functions.ServiceBus;

public class TimeOffRequested
{
	private readonly ICosmosContext _cosmosContext;

	public TimeOffRequested(ICosmosContext cosmosContext)
	{
		_cosmosContext = cosmosContext;
	}

	[Function(nameof(TimeOffRequested))]
	public async Task Run([ServiceBusTrigger("time-off-requested", Connection = "BananaTracks:Connections:ServiceBus")] TimeOffRequestedMessage message)
	{
	}
}
