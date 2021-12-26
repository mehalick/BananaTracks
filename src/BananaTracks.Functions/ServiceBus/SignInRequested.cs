using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace BananaTracks.Functions.ServiceBus;

public class SignInRequested
{
	private readonly ILogger _logger;

	public SignInRequested(ILoggerFactory loggerFactory)
	{
		_logger = loggerFactory.CreateLogger<SignInRequested>();
	}

	[Function(nameof(SignInRequested))]
	public void Run([ServiceBusTrigger("signin-requested", Connection = "BananaTracks:Connections:ServiceBus")] string myQueueItem)
	{
		_logger.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");
	}
}
