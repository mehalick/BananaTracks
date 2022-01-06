namespace BananaTracks.Domain.Entities;

public abstract class EntityBase
{
	public Guid Id { get; private init; } = Guid.NewGuid();

	public Guid TenantId { get; private init; }

	public DateTime CreatedAt { get; private init; } = DateTime.UtcNow;
	public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;

	protected EntityBase()
	{
	}

	protected EntityBase(Guid tenantId)
	{
		TenantId = tenantId;
	}
}
