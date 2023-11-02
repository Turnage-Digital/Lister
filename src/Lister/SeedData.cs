using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Serilog;

namespace Lister;

internal class SeedData
{
    public static void EnsureSeedData(WebApplication app)
    {
        Log.Information("Seeding database...");

        using var scope = app.Services
            .GetRequiredService<IServiceScopeFactory>()
            .CreateScope();
        var userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<IdentityUser>>();

        var heath = userManager.FindByNameAsync("heath").Result;
        if (heath == null)
        {
            CreateUser(userManager, "heath", "heath@email.com", "Pass123$", "Heath Turnage",
                "Heath", "Turnage", "https://thingman.com");
        }
        else
        {
            Log.Debug("heath already exists");
        }

        var erika = userManager.FindByNameAsync("erika").Result;
        if (erika == null)
        {
            CreateUser(userManager, "erika", "erika@email.com", "Pass123$", "Erika Turnage",
                "Erika", "Turnage", "https://thingman.com");
        }
        else
        {
            Log.Debug("erika already exists");
        }

        Log.Information("Done seeding database. Exiting.");
    }

    private static void CreateUser(
        UserManager<IdentityUser> userManager,
        string userName,
        string email,
        string password,
        string name,
        string givenName,
        string familyName,
        string website
    )
    {
        var user = new IdentityUser
        {
            UserName = userName,
            Email = email,
            EmailConfirmed = true
        };

        var result = userManager.CreateAsync(user, password).Result;
        if (!result.Succeeded)
        {
            throw new Exception(result.Errors.First().Description);
        }

        result = userManager.AddClaimsAsync(user, new Claim[]
        {
            new(JwtClaimTypes.Name, name),
            new(JwtClaimTypes.GivenName, givenName),
            new(JwtClaimTypes.FamilyName, familyName),
            new(JwtClaimTypes.WebSite, website)
        }).Result;
        if (!result.Succeeded)
        {
            throw new Exception(result.Errors.First().Description);
        }

        Log.Debug($"{userName} created");
    }

    private static class JwtClaimTypes
    {
        public const string Name = "name";
        public const string GivenName = "given_name";
        public const string FamilyName = "family_name";
        public const string WebSite = "website";
    }
}