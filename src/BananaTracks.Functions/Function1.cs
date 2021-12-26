using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;

namespace BananaTracks.Functions;

public class Function1
{
	private readonly IConfiguration _configuration;

	public Function1(IConfiguration configuration)
	{
		_configuration = configuration;
	}

	[Function("Function1")]
	public string Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequestData req)
	{
		var value = _configuration["BananaTracks:Tenants:0:Name"];

		if (string.IsNullOrWhiteSpace(value))
		{
			value = "nope";
		}

		return value;
	}
}
