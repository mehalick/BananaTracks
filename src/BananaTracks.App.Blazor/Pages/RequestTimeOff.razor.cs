namespace BananaTracks.App.Blazor.Pages;

public partial class RequestTimeOff : AppComponentBase
{
	public FormModel Model { get; set; } = new();

	public class FormModel
	{
		[Required]
		public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Today);
	}

	private async Task HandleValidSubmit()
	{
		await TimeOffServiceClient.RequestTimeOffAsync(new()
		{
			Date = Model.Date.ToTimestamp()
		});

		NavigateTo("/", "Time off successfully requested.");
	}
}
