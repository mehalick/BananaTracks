using Microsoft.Extensions.Configuration;
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
			.ConfigureServices(services =>
			{
				services.AddAzureAppConfiguration();
			})
			.Build();

		await host.RunAsync();
	}
}
