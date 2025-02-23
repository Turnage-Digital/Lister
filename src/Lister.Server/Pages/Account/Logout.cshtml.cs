using Lister.Users.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Lister.Server.Pages.Account;

[AllowAnonymous]
public class LogoutModel(SignInManager<User> signInManager) : PageModel
{
    public async Task<IActionResult> OnGet()
    {
        if (User.Identity is not null && User.Identity.IsAuthenticated)
        {
            return Page();
        }

        await signInManager.SignOutAsync();
        return RedirectToPage();
    }
}