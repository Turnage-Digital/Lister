using Lister.Users.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Serilog;

namespace Lister.App.Server.Services;

public class StubEmailSender : IEmailSender<User>
{
    public Task SendConfirmationLinkAsync(User user, string email, string confirmationLink)
    {
        Log.Information("Email confirmation link for {Email}: {ConfirmationLink}", email, confirmationLink);
        return Task.CompletedTask;
    }

    public Task SendPasswordResetLinkAsync(User user, string email, string resetLink)
    {
        // Convert the resetLink to our frontend format
        var uri = new Uri(resetLink);
        var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
        var code = query["code"];
        
        var frontendResetLink = $"http://localhost:3000/reset-password?email={Uri.EscapeDataString(email)}&code={Uri.EscapeDataString(code ?? "")}";
        
        Log.Information("Password reset link for {Email}: {ResetLink}", email, frontendResetLink);
        return Task.CompletedTask;
    }

    public Task SendPasswordResetCodeAsync(User user, string email, string resetCode)
    {
        Log.Information("Password reset code for {Email}: {ResetCode}", email, resetCode);
        return Task.CompletedTask;
    }
}