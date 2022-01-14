namespace BananaTracks.App.Services;

public class TimeOffService : Shared.Protos.TimeOffService.TimeOffServiceBase
{
	private readonly Guid _tenantId;
	private readonly IServiceBusProvider _serviceBusProvider;
	private readonly ICosmosContext _cosmosContext;

	public TimeOffService(ITenantService tenantService, IServiceBusProvider serviceBusProvider, ICosmosContext cosmosContext)
	{
		_tenantId = tenantService.Tenant.Id;
		_serviceBusProvider = serviceBusProvider;
		_cosmosContext = cosmosContext;
	}

	public override async Task<Empty> RequestTimeOff(RequestTimeOffRequest request, ServerCallContext context)
	{
		await _serviceBusProvider.Send(new TimeOffRequestedMessage
		{
			TenantId = _tenantId,
			UserId = context.GetUserId(),
			Date = request.Date.ToDateOnly()
		}, context.CancellationToken);

		return new();
	}

	public override async Task<GetTimeOffByTeamReply> GetTimeOffByTeam(GetTimeOffByTeamRequest request, ServerCallContext context)
	{
		var timeOff = await _cosmosContext.TimeOff
			.AsNoTracking()
			.WithPartitionKey(_tenantId.ToString())
			.Where(i => i.TeamId == Guid.Parse(request.TeamId))
			.OrderBy(i => i.Date)
			.ToListAsync(context.CancellationToken);

		var reply = new GetTimeOffByTeamReply();

		reply.Items.AddRange(timeOff.Select(i => new TimeOffItem
		{
			Id = i.Id.ToString(),
			UserId = i.UserId.ToString(),
			UserName = i.UserName,
			Date = i.Date.ToTimestamp(),
			Status = (Shared.Protos.TimeOffStatus)i.Status,
			Type = (Shared.Protos.TimeOffType)i.Type
		}));

		return reply;
	}
}
