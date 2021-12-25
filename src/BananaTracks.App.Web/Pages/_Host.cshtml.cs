namespace BananaTracks.App.Web.Pages;

public class HostModel : PageModel
{
	public string Test { get; }

	public HostModel(ITenantService tenantService)
	{
		Test = tenantService.Tenant.Name;
	}

	public void OnGet()
	{
	}
}
