namespace BananaTracks.Domain.Configuration;

public class AppSettings
{
	public Dictionary<string, Tenant> HostTenants { get; set; } = new Dictionary<string, Tenant>();

	private Tenant[] _tenants = null!;

	public Tenant[] Tenants
	{
		get => _tenants;
		set
		{
			_tenants = value;

			foreach (var tenant in _tenants)
			{
				HostTenants.Add(tenant.Host, tenant);
			}
		}
	}
}
