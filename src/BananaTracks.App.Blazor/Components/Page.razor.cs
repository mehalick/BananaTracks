namespace BananaTracks.App.Blazor.Components;

public partial class Page : ComponentBase
{
	[Parameter]
	public bool IsLoading { get; set; }

	[Parameter]
	public RenderFragment ChildContent { get; set; } = null!;

	[Inject]
	protected NavigationManager NavigationManager { get; set; } = null!;

	protected MarkupString StatusMessage { get; set; }

	protected override void OnInitialized()
	{
		var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);

		if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("success", out var message))
		{
			ShowSuccess(DecodeMessage(message.First()));
		}

		base.OnInitialized();
	}

	public void ShowSuccess(string message)
	{
		StatusMessage = new MarkupString(message);

		//await JsRuntime.InvokeVoidAsync("showToast");
	}

	public static string DecodeMessage(string message)
	{
		var decodedBytes = Base64UrlTextEncoder.Decode(message);

		return System.Text.Encoding.UTF8.GetString(decodedBytes);
	}
}
