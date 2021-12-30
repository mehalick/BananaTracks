using BananaTracks.App.Shared.Protos;
using Google.Protobuf.WellKnownTypes;

namespace BananaTracks.App.Blazor.Pages;

public partial class Index
{
	[Inject]
	protected UserService.UserServiceClient UserServiceClient { get; set; } = null!;

	public GetUserReply? User { get; set; }

	protected override async Task OnInitializedAsync()
	{
		User = await UserServiceClient.GetUserAsync(new Empty());
	}
}
