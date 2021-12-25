using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace BananaTracks.Functions;

internal class Program
{
	public static async Task Main()
	{
		var host = new HostBuilder().Build();

		await host.RunAsync();
	}
}
