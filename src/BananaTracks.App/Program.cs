using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Serilog;
using Serilog.Events;

namespace BananaTracks.App;

internal class Program
{
	public static async Task Main(params string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		builder.Host.UseSerilog((_, config) => config
			.MinimumLevel.Debug()
			.MinimumLevel.Override("Microsoft", LogEventLevel.Information)
			.MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
			.Enrich.FromLogContext()
			.WriteTo.Console()
			.WriteTo.ApplicationInsights(TelemetryConfiguration.CreateDefault(), TelemetryConverter.Traces));

		builder.Host.ConfigureAppConfiguration((context, config) =>
		{
			if (context.HostingEnvironment.IsDevelopment())
			{
				return;
			}

			var settings = config.Build();
			var connection = settings["BananaTracks:Connections:AppConfig"];

			config.AddAzureAppConfiguration(options =>
			{
				options.Connect(connection)
					.Select("BananaTracks*")
					.Select("BananaTracks*", "BananaTracks")
					.ConfigureRefresh(refresh => refresh.Register("BananaTracks:_version", true));
			});
		});

		builder.Services.AddAzureAppConfiguration();
		builder.Services.AddOptions();
		builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("BananaTracks"));

		builder.Services.AddGrpc();

		builder.Services.AddRazorPages(options =>
		{
			options.Conventions.AllowAnonymousToFolder("/auth");
			options.Conventions.AddPageRoute("/_Host", "{*url}");
		});

		builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

		builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
			.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
			{
				options.LoginPath = "/auth/send-link";
				options.LogoutPath = "/auth/sign-out";
				options.Events = new CookieAuthenticationEvents
				{
					OnRedirectToLogin = redirectContext =>
					{
						redirectContext.Response.Redirect(RemoveQueryString(redirectContext));
						return Task.CompletedTask;
					}
				};
			});

		builder.Services.AddAuthorization(options =>
			options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());

		builder.Services.AddHttpContextAccessor();

		builder.Services.AddDependencies(builder.Configuration);

		var app = builder.Build();

		if (app.Environment.IsDevelopment())
		{
			app.UseWebAssemblyDebugging();
		}
		else
		{
			app.UseAzureAppConfiguration();
			app.UseExceptionHandler("/Error");
			app.UseHsts();
			app.UseHttpsRedirection();
		}

		app.UseCookiePolicy(new CookiePolicyOptions
		{
			MinimumSameSitePolicy = SameSiteMode.Strict,
		});

		app.UseSerilogRequestLogging();

		app.UseBlazorFrameworkFiles();
		app.UseStaticFiles();

		app.UseRouting();

		app.UseAuthentication();
		app.UseAuthorization();

		app.UseGrpcWeb();

		app.MapRazorPages();
		app.MapGrpcService<Services.UserService>().EnableGrpcWeb();
		app.MapFallbackToFile("/_Host");

		await app.RunAsync();
	}

	private static string RemoveQueryString(RedirectContext<CookieAuthenticationOptions> redirectContext)
	{
		UriHelper.FromAbsolute(redirectContext.RedirectUri, out var scheme, out var host, out var path, out _, out _);
		return UriHelper.BuildAbsolute(scheme, host, path);
	}
}
