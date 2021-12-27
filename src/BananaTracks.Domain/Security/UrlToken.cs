namespace BananaTracks.Domain.Security;

public class UrlToken
{
	public long Timestamp { get; set; }
	public string Hash { get; set; } = "";

	public UrlToken()
	{
	}

	public UrlToken(DateTime timestamp, string hash)
	{
		Timestamp = timestamp.Ticks;
		Hash = hash;
	}

	public UrlToken(long timestamp, string hash)
	{
		Timestamp = timestamp;
		Hash = hash;
	}
}
