using BananaTracks.Domain.Messaging;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;

namespace BananaTracks.Functions;

public class Function1
{
	private readonly IConfiguration _configuration;
	private readonly IEmailProvider _emailProvider;

	public Function1(IConfiguration configuration, IEmailProvider emailProvider)
	{
		_configuration = configuration;
		_emailProvider = emailProvider;
	}

	[Function(nameof(Function1))]
	public async Task<string> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequestData req)
	{
		var value = _configuration["BananaTracks:Connections:ServiceBus"];

		if (string.IsNullOrWhiteSpace(value))
		{
			value = "nope";
		}

		await _emailProvider.Send("andy.mehalick@outlook.com", "Test Email", "This is a test",
			"This is a <strong>test</strong>!");

		return value;
	}
}
