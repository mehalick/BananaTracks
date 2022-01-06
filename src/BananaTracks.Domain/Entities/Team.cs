namespace BananaTracks.Domain.Entities;

public class Team : EntityBase
{
	[Required]
	public string Name { get; private set; } = "";

	public Guid ManagerId { get; set; }

	private Team()
	{
		// required for EF
	}

	public Team(Guid tenantId, string name) : base(tenantId)
	{
		Name = name.Trim();
	}
}
