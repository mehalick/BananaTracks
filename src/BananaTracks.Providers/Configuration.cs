using System.Text.Json;
using Azure.Messaging.ServiceBus;
using BananaTracks.Providers.CosmosDb;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BananaTracks.Providers;

public static class Configuration
{
	public static void AddDependencies(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddDbContext<ICosmosContext, CosmosContext>(options => options.UseCosmos(configuration["BananaTracks:Connections:CosmosDb"], "banana-tracks"));

		services.AddSingleton(_ => new ServiceBusClient(configuration["BananaTracks:Connections:ServiceBus"]));

		services.AddSingleton<IEmailProvider, SendGridProvider>();
		services.AddSingleton<IServiceBusProvider, ServiceBusProvider>();
		services.AddSingleton<UrlSecurity>();

		services.AddScoped<ITenantService, TenantService>();

		services.Configure<JsonSerializerOptions>(options =>
		{
			options.Converters.Add(new DateOnlyJsonConverter());
			options.Converters.Add(new DateOnlyNullableJsonConverter());
		});
	}
}
