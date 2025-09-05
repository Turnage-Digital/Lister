using Microsoft.AspNetCore.Identity.UI.Services;
using Serilog;

namespace Lister.App.Server.Services;

public class StubEmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        Log.Information("Email sent to {Email} with subject {Subject}: {Message}", email, subject, htmlMessage);
        return Task.CompletedTask;
    }
}