using System.ComponentModel.DataAnnotations;

namespace BananaTracks.Domain;

public class Team : EntityBase
{
	[Required]
	public string Name { get; private set; } = "";

	public Guid ManagerId { get; set; }

	private Team()
	{
		// required for EF
	}

	public Team(Guid tenantId, string name)
	{
		TenantId = tenantId;
		Name = name.Trim();
	}
}
