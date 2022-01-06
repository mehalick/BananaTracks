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
}
