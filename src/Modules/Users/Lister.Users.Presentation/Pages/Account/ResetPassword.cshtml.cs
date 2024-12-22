using System.ComponentModel.DataAnnotations;
using System.Text;
using Lister.Users.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace Lister.Users.Presentation.Pages.Account;

[AllowAnonymous]
public class ResetPassword(UserManager<User> userManager) : PageModel
{
    public string? Code { get; set; }

    public string? ReturnUrl { get; set; }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public IActionResult OnGet(string? code = null, string? returnUrl = null)
    {
        if (code == null) return BadRequest("A code must be supplied for password reset.");

        Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        ReturnUrl = returnUrl ?? Url.Content("~/");
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string? code = null, string? returnUrl = null)
    {
        if (code == null) return BadRequest("A code must be supplied for password reset.");

        Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        ReturnUrl = returnUrl ?? Url.Content("~/");

        if (!ModelState.IsValid) return Page();

        var user = await userManager.FindByNameAsync(Input.UserName!);
        if (user == null) return RedirectToPage(nameof(ResetPasswordConfirmation), new { ReturnUrl });

        var result = await userManager.ResetPasswordAsync(user, Code, Input.Password!);
        if (result.Succeeded) return RedirectToPage(nameof(ResetPasswordConfirmation), new { ReturnUrl });

        foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);

        return Page();
    }

    public class InputModel
    {
        [Required]
        public string? UserName { get; init; }

        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; init; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string? ConfirmPassword { get; init; }
    }
}