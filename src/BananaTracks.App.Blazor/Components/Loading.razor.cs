namespace BananaTracks.App.Blazor.Components;

public partial class Loading : ComponentBase
{
	[Parameter]
	public bool IsLoading { get; set; }

	[Parameter]
	public LoadingStyle Style { get; set; } = LoadingStyle.None;

	[Parameter]
	public RenderFragment ChildContent { get; set; } = null!;
}

public enum LoadingStyle
{
	Panel, None
}
