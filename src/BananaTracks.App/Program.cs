using Joonasw.AspNetCore.SecurityHeaders;
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
		builder.Services.AddCsp(nonceByteAmount: 32);

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
		}

		app.UseSerilogRequestLogging();

		app.UseCookiePolicy(new() { MinimumSameSitePolicy = SameSiteMode.Strict });

		app.UseHsts(new(TimeSpan.FromDays(365), includeSubDomains: true, preload: false));
		app.UseHttpsRedirection();

		app.UseCsp(csp =>
		{
			csp.ByDefaultAllow
				.FromSelf();

			csp.AllowScripts
				.AllowUnsafeEval()
				.FromSelf()
				.From("'sha256-v8v3RKRPmN4odZ1CWM5gw80QKPCCWMcpNeOmimNL2AA='") // Blazor Framework
				.From("'sha256-3Ey30PJkNcf9LrK7CIqrujoq79a+uJqKgYsaBDj15Eo='") // Font Awesome
				.From("https://kit.fontawesome.com")
				.AddNonce();

			csp.AllowStyles
				.FromSelf()
				.From("'sha256-ixVUGs3ai0rMA0pgIVBN0KVlYbQip7/5SGmnUwJPNqE='") // Font Awesome
				.From("https://cdnjs.cloudflare.com");

			csp.AllowImages
				.FromSelf()
				.From("data:")
				.From("https://ojs.azureedge.net");

			csp.AllowAudioAndVideo
				.FromNowhere();

			csp.AllowFrames
				.FromNowhere();

			csp.AllowConnections
				.To("ws:")
				.To("https://ka-p.fontawesome.com")
				.ToSelf();

			csp.AllowFonts
				.FromSelf()
				.From("https://kit.fontawesome.com");

			csp.AllowPlugins
				.FromNowhere();

			csp.AllowFraming
				.FromNowhere();
		});

		app.UseBlazorFrameworkFiles();
		app.UseStaticFiles();

		app.UseRouting();

		app.UseAuthentication();
		app.UseAuthorization();

		app.UseGrpcWeb();

		app.MapRazorPages();
		app.MapGrpcService<Services.TimeOffService>().EnableGrpcWeb();
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
