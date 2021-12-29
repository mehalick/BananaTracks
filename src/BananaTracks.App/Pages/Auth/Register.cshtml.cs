using System.ComponentModel.DataAnnotations;
using BananaTracks.App.Extensions;
using BananaTracks.Domain;
using Microsoft.EntityFrameworkCore;

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
		var x = await _cosmosContext.Users.Where(x => x.TenantId == _tenantId).FirstOrDefaultAsync(cancellationToken);

		var y = await _cosmosContext.Users
			.AsNoTracking()
			.WithPartitionKey(_tenantId.ToString())
			.Select(i => i.Id)
			.FirstOrDefaultAsync(cancellationToken);

		var z = await _cosmosContext.Users.WithPartitionKey(_tenantId.ToString()).CountAsync(cancellationToken);

		return Page();
	}

	public async Task<IActionResult> OnPost(CancellationToken cancellationToken)
	{
		if (!ModelState.IsValid)
		{
			return Page();
		}

		var user = new User(_tenantId, Email, Name, Claims.Administrator);
		var team = new Team(_tenantId, Team)
		{
			ManagerId = user.Id
		};

		user.TeamId = team.Id;

		await _cosmosContext.Users.AddAsync(user, cancellationToken);
		await _cosmosContext.Teams.AddAsync(team, cancellationToken);
		await _cosmosContext.SaveChangesAsync(cancellationToken);

		await HttpContext.SignInAsync(_tenantId, user);

		return Redirect("/");
	}
}
