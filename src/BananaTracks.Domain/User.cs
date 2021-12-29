using System.ComponentModel.DataAnnotations;

namespace BananaTracks.Domain;

public class User : EntityBase
{
	[Required]
	public string Email { get; private set; } = "";

	[Required]
	public string Name { get; private set; } = "";

	public Claims Claims { get; private set; } = Claims.None;

	public Guid TeamId { get; set; }

	private User()
	{
		// required for EF
	}

	public User(Guid tenantId, string email, string name, Claims claims)
	{
		TenantId = tenantId;
		Email = email.ToLowerInvariant().Trim();
		Name = name.Trim();
		Claims = claims;
	}
}
