using System.Security.Claims;
using Lister.Core.Enums;
using Lister.Core.SqlDB.Entities;
using Lister.Core.ValueObjects;
using Lister.Domain;
using Microsoft.AspNetCore.Identity;
using Serilog;

namespace Lister;

internal static class SeedData
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
                "Heath", "Turnage", "https://lister.com");
            heath = userManager.FindByNameAsync("heath").Result;
        }
        else
        {
            Log.Debug("heath already exists");
        }

        var erika = userManager.FindByNameAsync("erika").Result;
        if (erika == null)
            CreateUser(userManager, "erika", "erika@email.com", "Pass123$", "Erika Turnage",
                "Erika", "Turnage", "https://lister.com");
        else
            Log.Debug("erika already exists");

        var listAggregate = scope.ServiceProvider
            .GetRequiredService<ListAggregate<ListEntity>>();

        var list = listAggregate.FindByNameAsync("Trailer Park Boys").Result;
        if (list == null)
        {
            list = listAggregate.CreateAsync(
                heath!.Id,
                "Trailer Park Boys",
                new[]
                {
                    new Status
                    {
                        Name = "Active",
                        Color = "#FFCA28"
                    },
                    new Status
                    {
                        Name = "Inactive",
                        Color = "#607d8b"
                    }
                },
                new[]
                {
                    new Column
                    {
                        Name = "Name",
                        Type = ColumnType.Text
                    },
                    new Column
                    {
                        Name = "Address",
                        Type = ColumnType.Text
                    },
                    new Column
                    {
                        Name = "City",
                        Type = ColumnType.Text
                    },
                    new Column
                    {
                        Name = "State",
                        Type = ColumnType.Text
                    },
                    new Column
                    {
                        Name = "Zip Code",
                        Type = ColumnType.Text
                    },
                    new Column
                    {
                        Name = "Date Of Birth",
                        Type = ColumnType.Date
                    }
                }
            ).Result;

            listAggregate.CreateItemAsync(list, heath.Id, new
                {
                    city = "Spring Hill",
                    name = "Ricky Lafluer",
                    state = "FL",
                    status = "Active",
                    address = "The Sh*tmobile",
                    zipCode = "34609",
                    dateOfBirth = DateTime.Parse("1/1/1970")
                        .ToUniversalTime()
                        .ToString("O")
                }
            ).Wait();

            listAggregate.CreateItemAsync(list, heath.Id, new
                {
                    city = "Spring Hill",
                    name = "Julian",
                    state = "FL",
                    status = "Active",
                    address = "101 Sunnyvale Lane",
                    zipCode = "34707",
                    dateOfBirth = DateTime.Parse("1/1/1970")
                        .ToUniversalTime()
                        .ToString("O")
                }
            ).Wait();

            listAggregate.CreateItemAsync(list, heath.Id, new
                {
                    city = "Spring Hill",
                    name = "Bubbles",
                    state = "FL",
                    status = "Active",
                    address = "The Shed",
                    zipCode = "34609",
                    dateOfBirth = DateTime.Parse("1/1/1970")
                        .ToUniversalTime()
                        .ToString("O")
                }
            ).Wait();

            listAggregate.CreateItemAsync(list, heath.Id, new
                {
                    city = "Columbia",
                    name = "Sh*tty Bill",
                    state = "SC",
                    status = "Active",
                    address = "202 No Way",
                    zipCode = "29212",
                    dateOfBirth = DateTime.Parse("2/1/1945")
                        .ToUniversalTime()
                        .ToString("O")
                }
            ).Wait();

            listAggregate.CreateItemAsync(list, heath.Id, new
                {
                    city = "Columbia",
                    name = "Ray Lafluer",
                    state = "SC",
                    status = "Inactive",
                    address = "The Dump",
                    zipCode = "29212",
                    dateOfBirth = DateTime.Parse("1/1/1950")
                        .ToUniversalTime()
                        .ToString("O")
                }
            ).Wait();

            listAggregate.CreateItemAsync(list, heath.Id, new
                {
                    city = "New York",
                    name = "Trevor",
                    state = "NY",
                    status = "Inactive",
                    address = "Unknown",
                    zipCode = "10001",
                    dateOfBirth = DateTime.Parse("1/1/1970")
                        .ToUniversalTime()
                        .ToString("O")
                }
            ).Wait();

            listAggregate.CreateItemAsync(list, heath.Id, new
                {
                    city = "Spring Hill",
                    name = "Corey",
                    state = "FL",
                    status = "Active",
                    address = "101 Other Way",
                    zipCode = "34609",
                    dateOfBirth = DateTime.Parse("1/1/1970")
                        .ToUniversalTime()
                        .ToString("O")
                }
            ).Wait();

            listAggregate.CreateItemAsync(list, heath.Id, new
                {
                    city = "Spring Hill",
                    name = "Lucy Lafluer",
                    state = " FL",
                    status = "Inactive",
                    address = "100 Maple Lane",
                    zipCode = "34609",
                    dateOfBirth = DateTime.Parse("1/1/1970")
                        .ToUniversalTime()
                        .ToString("O")
                }
            ).Wait();

            listAggregate.CreateItemAsync(list, heath.Id, new
                {
                    city = "Spring Hill",
                    name = "Sara",
                    state = "FL",
                    status = "Active",
                    address = "100 Maple Lane",
                    zipCode = "34609",
                    dateOfBirth = DateTime.Parse("1/1/1970")
                        .ToUniversalTime()
                        .ToString("O")
                }
            ).Wait();

            listAggregate.CreateItemAsync(list, heath.Id, new
                {
                    city = "Spring Hill",
                    name = "Trinity Lafluer",
                    state = "FL",
                    status = "Active",
                    address = "100 Maple Lane",
                    zipCode = "34609",
                    dateOfBirth = DateTime.Parse("1/1/1990")
                        .ToUniversalTime()
                        .ToString("O")
                }
            ).Wait();
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
        if (!result.Succeeded) throw new Exception(result.Errors.First().Description);

        result = userManager.AddClaimsAsync(user, new Claim[]
        {
            new(JwtClaimTypes.Name, name),
            new(JwtClaimTypes.GivenName, givenName),
            new(JwtClaimTypes.FamilyName, familyName),
            new(JwtClaimTypes.WebSite, website)
        }).Result;
        if (!result.Succeeded) throw new Exception(result.Errors.First().Description);

        Log.Debug($"{userName} created");
    }

    private static long GetJavascriptTimestamp(string dateTime)
    {
        var parsed = DateTime.Parse(dateTime);
        var retval = (long)(parsed - new DateTime(1970, 1, 1))
            .TotalMilliseconds;
        return retval;
    }

    private static class JwtClaimTypes
    {
        public const string Name = "name";
        public const string GivenName = "given_name";
        public const string FamilyName = "family_name";
        public const string WebSite = "website";
    }
}