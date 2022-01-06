using BananaTracks.App.Shared.Protos;
using Google.Protobuf.WellKnownTypes;

namespace BananaTracks.App.Blazor.Pages;

public partial class Index : AppComponentBase
{
	public GetUserReply? User { get; set; }

	protected override async Task OnInitializedAsync()
	{
		User = await UserServiceClient.GetUserAsync(new Empty());
	}
}
