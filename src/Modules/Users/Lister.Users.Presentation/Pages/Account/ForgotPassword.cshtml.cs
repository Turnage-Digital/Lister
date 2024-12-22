using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using Lister.Users.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace Lister.Users.Presentation.Pages.Account;

[AllowAnonymous]
public class ForgotPassword(
    UserManager<User> userManager,
    IEmailSender emailSender
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

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        ReturnUrl = returnUrl ?? Url.Content("~/");

        if (!ModelState.IsValid) return Page();

        var user = await userManager.FindByNameAsync(Input.UserName!);
        if (user == null) return RedirectToPage(nameof(ForgotPasswordConfirmation));

        var code = await userManager.GeneratePasswordResetTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        var callbackUrl = Url.Page(
            "/Account/ResetPassword",
            null,
            new { code, ReturnUrl },
            Request.Scheme);

        await emailSender.SendEmailAsync(
            user.Email!,
            "Reset Password",
            $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl!)}'>clicking here</a>.");

        return RedirectToPage(nameof(ForgotPasswordConfirmation), new { ReturnUrl });
    }

    public class InputModel
    {
        [Required]
        public string? UserName { get; init; }
    }
}