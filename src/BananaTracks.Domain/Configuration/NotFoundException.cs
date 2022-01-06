namespace BananaTracks.Domain.Configuration;

public class NotFoundException<T> : Exception
{
	public NotFoundException() : base($"No '{typeof(T).Name}' found given the filter criteria provided to this action.")
	{
	}
}
