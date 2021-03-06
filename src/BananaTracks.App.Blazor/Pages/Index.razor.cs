namespace BananaTracks.App.Blazor.Pages;

public partial class Index : AppComponentBase
{
	public GetTimeOffByTeamReply? TimeOff { get; set; }

	protected override async Task OnInitializedAsync()
	{
		var user = await UserServiceClient.GetUserAsync(new());

		TimeOff = await TimeOffServiceClient.GetTimeOffByTeamAsync(new()
		{
			TeamId = user.TeamId
		});
	}
}
