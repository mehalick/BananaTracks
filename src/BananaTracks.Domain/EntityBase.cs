namespace BananaTracks.Domain;

public abstract class EntityBase
{
	public Guid Id { get; private set; } = Guid.NewGuid();

	public Guid TenantId { get; set; }

	public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
	public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;
}
