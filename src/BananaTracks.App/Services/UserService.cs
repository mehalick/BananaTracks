using BananaTracks.App.Extensions;
using BananaTracks.App.Shared.Extensions;
using BananaTracks.App.Shared.Protos;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;

namespace BananaTracks.App.Services;

public class UserService : Shared.Protos.UserService.UserServiceBase
{
	private readonly Guid _tenantId;
	private readonly ICosmosContext _cosmosContext;

	public UserService(ITenantService tenantService, ICosmosContext cosmosContext)
	{
		_tenantId = tenantService.Tenant.Id;
		_cosmosContext = cosmosContext;
	}

	public override async Task<GetUserReply> GetUser(Empty request, ServerCallContext context)
	{
		var user = await _cosmosContext.Users
			.AsNoTracking()
			.WithPartitionKey(_tenantId.ToString())
			.Where(i => i.Id == context.GetUserId())
			.SingleAsync(context.CancellationToken);

		return new GetUserReply
		{
			Id = user.Id.ToString(),
			Email = user.Email,
			Name = user.Name,
			TeamId = user.TeamId.ToString(),
			TeamName = user.TeamName,
			StartDate = user.StartDate.ToTimestamp(),
			ResetDate = user.ResetDate.ToTimestamp()
		};
	}
}
