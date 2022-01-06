using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddSingleton(services => new UserService.UserServiceClient(Channel(services)));
builder.Services.AddSingleton(services => new TimeOffService.TimeOffServiceClient(Channel(services)));

await builder.Build().RunAsync();

static GrpcChannel Channel(IServiceProvider services)
{
	var baseUri = services.GetRequiredService<NavigationManager>().BaseUri;
	var httpClient = new HttpClient(new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler()));

	return GrpcChannel.ForAddress(baseUri, new()
	{
		HttpClient = httpClient
	});
}
