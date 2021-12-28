namespace BananaTracks.Domain;

public class User : EntityBase
{
	public string Email { get; private set; } = "";
	public string Name { get; private set; } = "";

	public Claims Claims { get; private set; } = Claims.None;

	private User()
	{
		// required for EF
	}

	public User(Guid tenantId, string email, string name, Claims claims)
	{
		TenantId = tenantId;
		Email = email;
		Name = name;
		Claims = claims;
	}
}

public abstract class EntityBase
{
	public Guid Id { get; private set; } = Guid.NewGuid();

	public Guid TenantId { get; set; }

	public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
	public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;
}

[Flags]
public enum Claims
{
	None = 0,
	Administrator = 1
}
