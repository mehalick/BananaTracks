using BananaTracks.Domain.Entities;
using BananaTracks.Domain.Extensions;

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
		var user = await _cosmosContext.Users
			.AsNoTracking()
			.WithPartitionKey(message.TenantId.ToString())
			.Where(i => i.Id == message.UserId)
			.SingleOrThrowAsync();

		var timeOff = new TimeOff(message.TenantId)
		{
			TeamId = user.TeamId,
			UserId = user.Id,
			Status = TimeOffStatus.Approved,
			Type = TimeOffType.Planned,
			Date = message.Date
		};

		await _cosmosContext.TimeOff.AddAsync(timeOff);
		await _cosmosContext.SaveChangesAsync();
	}
}
