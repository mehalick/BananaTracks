namespace BananaTracks.Domain;

public class User : EntityBase
{
	public string Name { get; private set; } = "";

	private User()
	{
		// required for EF
	}

	public User(string name)
	{
		Name = name;
	}
}

public abstract class EntityBase
{
	public Guid Id { get; private set; } = Guid.NewGuid();

	public Guid TenantId { get; set; }

	public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
	public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;
}
