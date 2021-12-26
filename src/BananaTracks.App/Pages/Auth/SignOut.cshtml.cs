using Microsoft.AspNetCore.Authentication;

namespace BananaTracks.App.Pages.Auth;

public class SignOutModel : PageModel
{
	public async Task<IActionResult> OnPost()
	{
		await HttpContext.SignOutAsync();

		return RedirectToPage(nameof(SignIn));
	}
}
