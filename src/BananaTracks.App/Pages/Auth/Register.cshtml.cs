namespace BananaTracks.App.Pages.Auth;

public class RegisterModel : PageModel
{
	[BindProperty]
	[Required, EmailAddress]
	public string Email { get; set; } = "andy.mehalick@outlook.com";

	[BindProperty]
	[Required]
	public string Name { get; set; } = "Andy";

	[BindProperty]
	[Required]
	public string Team { get; set; } = "Developers";

	private readonly Guid _tenantId;
	private readonly ICosmosContext _cosmosContext;

	public RegisterModel(ITenantService tenantService, ICosmosContext cosmosContext)
	{
		_tenantId = tenantService.Tenant.Id;
		_cosmosContext = cosmosContext;
	}

	public async Task<IActionResult> OnGet(CancellationToken cancellationToken)
	{
		var userCount = await _cosmosContext.Users.WithPartitionKey(_tenantId.ToString()).CountAsync(cancellationToken);

		if (userCount > 0)
		{
			return Content("Cannot register, team and admin already set for current tenant.");
		}

		return Page();
	}

	public async Task<IActionResult> OnPost(CancellationToken cancellationToken)
	{
		if (!ModelState.IsValid)
		{
			return Page();
		}

		var team = new Team(_tenantId, Team);

		var user = new User(_tenantId, Email, Name)
		{
			Claims = Claims.Administrator,
			TeamId = team.Id,
			TeamName = team.Name
		};

		team.ManagerId = user.Id;

		await _cosmosContext.Teams.AddAsync(team, cancellationToken);
		await _cosmosContext.Users.AddAsync(user, cancellationToken);
		await _cosmosContext.SaveChangesAsync(cancellationToken);

		await HttpContext.SignInAsync(_tenantId, user);

		return Redirect("/");
	}
}
