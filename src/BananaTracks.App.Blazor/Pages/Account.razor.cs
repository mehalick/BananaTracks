namespace BananaTracks.App.Blazor.Pages;

public partial class Account : AppComponentBase
{
	public GetUserReply? User { get; set; }

	protected override async Task OnInitializedAsync()
	{
		User = await UserServiceClient.GetUserAsync(new());
	}
}
