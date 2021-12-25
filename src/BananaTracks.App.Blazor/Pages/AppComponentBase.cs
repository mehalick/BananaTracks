namespace BananaTracks.App.Blazor.Pages;

public class AppComponentBase : ComponentBase
{
	[Inject]
	protected NavigationManager NavigationManager { get; set; } = null!;

	public static string EncodeMessage(string message)
	{
		var encodedBytes = System.Text.Encoding.UTF8.GetBytes(message);

		return Base64UrlTextEncoder.Encode(encodedBytes);
	}

	public void NavigateTo(string uri, string message)
	{
		NavigationManager.NavigateTo(QueryHelpers.AddQueryString(uri, "success", EncodeMessage(message)));
	}
}
