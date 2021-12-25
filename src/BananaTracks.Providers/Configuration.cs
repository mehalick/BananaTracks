using BananaTracks.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BananaTracks.Providers;

public static class Configuration
{
	public static void AddDependencies(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddDbContext<ICosmosContext, CosmosContext>(options => options.UseCosmos(configuration["BananaTracks:Connections:CosmosDb"], "banana-tracks"));
		services.AddScoped<ITenantService, TenantService>();
	}
}
