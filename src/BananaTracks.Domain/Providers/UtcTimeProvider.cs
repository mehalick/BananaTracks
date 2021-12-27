namespace BananaTracks.Domain.Providers;

public class UtcTimeProvider : ITimeProvider
{
	public DateTime Now()
	{
		return DateTime.UtcNow;
	}
}
