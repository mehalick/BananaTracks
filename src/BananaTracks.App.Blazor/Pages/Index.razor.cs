namespace BananaTracks.App.Blazor.Pages;

public partial class Index : AppComponentBase
{
	public GetUserReply? User { get; set; }
	public GetTimeOffByTeamReply? TimeOff { get; set; }

	protected override async Task OnInitializedAsync()
	{
		User = await UserServiceClient.GetUserAsync(new());

		TimeOff = await TimeOffServiceClient.GetTimeOffByTeamAsync(new()
		{
			TeamId = User.TeamId
		});
	}
}
