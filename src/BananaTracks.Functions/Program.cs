using System.Reflection;
using BananaTracks.Providers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BananaTracks.Functions;

public class Program
{
	public static async Task Main()
	{
		var host = new HostBuilder()
			.ConfigureFunctionsWorkerDefaults()
			.ConfigureAppConfiguration(config =>
			{
				config.AddUserSecrets(Assembly.GetExecutingAssembly(), true);

				var settings = config.Build();
				var connection = settings["BananaTracks:Connections:AppConfig"];

				config.AddAzureAppConfiguration(options =>
				{
					options.Connect(connection)
						.Select("BananaTracks*")
						.Select("BananaTracks*", "BananaTracks")
						.ConfigureRefresh(refresh => refresh.Register("BananaTracks:_version", true));
				});
			})
			.ConfigureServices((context, services) =>
			{
				services.AddOptions();
				services.Configure<AppSettings>(context.Configuration.GetSection("BananaTracks"));

				var serviceProvider = services.BuildServiceProvider();
				var configuration = serviceProvider.GetRequiredService<IConfiguration>();

				services.AddDependencies(configuration);
			})
			.Build();

		await host.RunAsync();
	}
}
