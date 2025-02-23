using System.ComponentModel.DataAnnotations;
using Lister.Users.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Lister.Server.Pages.Account;

[AllowAnonymous]
public class LoginModel(
    UserManager<User> userManager,
    SignInManager<User> signInManager
)
    : PageModel
{
    public string? ReturnUrl { get; set; }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public IActionResult OnGet(string? returnUrl = null)
    {
        ReturnUrl = returnUrl ?? Url.Content("~/");
        return Page();
    }

    public async Task<IActionResult> OnPost(string? returnUrl = null)
    {
        ReturnUrl = returnUrl ?? Url.Content("~/");

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = await userManager.FindByEmailAsync(Input.EmailAddress!);
        if (user != null)
        {
            var result = await signInManager.CheckPasswordSignInAsync(user, Input.Password!, true);
            if (result.Succeeded)
            {
                await signInManager.SignInAsync(user, true);
                return Redirect(ReturnUrl);
            }
        }

        ModelState.AddModelError(string.Empty, "Invalid email address or password");

        return Page();
    }

    public class InputModel
    {
        [Required(ErrorMessage = "The Email Address field is required.")]
        [EmailAddress(ErrorMessage = "The Email Address field is not a valid e-mail address.")]
        public string? EmailAddress { get; init; }

        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; init; }
    }
}