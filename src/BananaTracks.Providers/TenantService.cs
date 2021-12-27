using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace BananaTracks.Providers;

public class TenantService : ITenantService
{
	private readonly IOptions<AppSettings> _appSettings;
	private readonly IHttpContextAccessor _httpContextAccessor;

	public TenantService(IOptions<AppSettings> appSettings, IHttpContextAccessor httpContextAccessor)
	{
		_appSettings = appSettings;
		_httpContextAccessor = httpContextAccessor;
	}

	private Tenant? _tenant;

	public Tenant Tenant
	{
		get
		{
			if (_tenant is not null)
			{
				return _tenant;
			}

			var host = _httpContextAccessor.HttpContext.Request.Host.ToString();

			return _tenant = _appSettings.Value.TenantsByHost[host];
		}
	}
}
