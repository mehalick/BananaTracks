namespace BananaTracks.Domain.Configuration;

public class AppSettings
{
	public Dictionary<Guid, Tenant> TenantsById { get; set; } = new Dictionary<Guid, Tenant>();
	public Dictionary<string, Tenant> TenantsByHost { get; set; } = new Dictionary<string, Tenant>();

	private Tenant[] _tenants = null!;

	public Tenant[] Tenants
	{
		get => _tenants;
		set
		{
			_tenants = value;

			foreach (var tenant in _tenants)
			{
				TenantsById.Add(tenant.Id, tenant);
				TenantsByHost.Add(tenant.Host, tenant);
			}
		}
	}
}
