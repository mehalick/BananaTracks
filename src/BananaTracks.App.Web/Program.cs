using Azure.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureAppConfiguration(config =>
{
	var settings = config.Build();
	var endpoint = settings["AppConfig:Endpoint"];

	if (string.IsNullOrWhiteSpace(endpoint))
	{
		return;
	}

	config.AddAzureAppConfiguration(options =>
	{
		options.Connect(new Uri(endpoint), new DefaultAzureCredential())
			.Select("BananaTracks*")
			.Select("BananaTracks*", "BananaTracks")
			.ConfigureRefresh(refresh => refresh.Register("BananaTracks:_version", true));
	});
});

builder.Services.AddOptions();
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("BananaTracks"));

builder.Services.AddRazorPages(options =>
{
	options.Conventions.AllowAnonymousToFolder("/account");
	options.Conventions.AddPageRoute("/_Host", "{*url}");
});

builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

builder.Services.AddHttpContextAccessor();

builder.Services.AddDependencies(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
	var services = scope.ServiceProvider;

	try
	{
		var context = services.GetRequiredService<ICosmosContext>();

		await context.Initialize();
	}
	catch (Exception ex)
	{
		var logger = services.GetRequiredService<ILogger<Program>>();
		logger.LogError(ex, "An error occurred while seeding the database.");
	}
}

if (app.Environment.IsDevelopment())
{
	app.UseWebAssemblyDebugging();
}
else
{
	app.UseExceptionHandler("/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.MapRazorPages();
app.MapFallbackToFile("/_Host");

app.Run();
