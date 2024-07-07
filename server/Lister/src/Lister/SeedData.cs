using System.Security.Claims;
using Bogus;
using Lister.Core.Enums;
using Lister.Core.SqlDB.Entities;
using Lister.Core.ValueObjects;
using Lister.Domain;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
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
        {
            CreateUser(userManager, "erika", "erika@email.com", "Pass123$", "Erika Turnage",
                "Erika", "Turnage", "https://lister.com");
        }
        else
            Log.Debug("erika already exists");

        var listAggregate = scope.ServiceProvider
            .GetRequiredService<ListAggregate<ListEntity, ItemEntity>>();

        var list = listAggregate.FindByNameAsync("Students").Result;
        if (list == null)
        {
            list = listAggregate.CreateAsync(
                heath!.Id,
                "Students",
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

            var faker = new Faker<Student>()
                .RuleFor(s => s.City, f => f.Person.Address.City)
                .RuleFor(s => s.Name, f => f.Person.FullName)
                .RuleFor(s => s.State, f => f.Person.Address.State)
                .RuleFor(s => s.Status, f => f.PickRandom("Active", "Inactive"))
                .RuleFor(s => s.Address, f => f.Person.Address.Street)
                .RuleFor(s => s.ZipCode, f => f.Address.ZipCode())
                .RuleFor(s => s.DateOfBirth, f => f.Person.DateOfBirth.ToString("O"));
            var students = faker.Generate(10000);
            
            listAggregate.AddListItemsAsync(list, heath.Id, students).Wait();
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

    private static class JwtClaimTypes
    {
        public const string Name = "name";
        public const string GivenName = "given_name";
        public const string FamilyName = "family_name";
        public const string WebSite = "website";
    }

    private class Student
    {
        [JsonProperty("city")]
        public string City { get; set; } = null!;

        [JsonProperty("name")]
        public string Name { get; set; } = null!;

        [JsonProperty("state")]
        public string State { get; set; } = null!;

        [JsonProperty("status")]
        public string Status { get; set; } = null!;

        [JsonProperty("address")]
        public string Address { get; set; } = null!;

        [JsonProperty("zipCode")]
        public string ZipCode { get; set; } = null!;

        [JsonProperty("dateOfBirth")]
        public string DateOfBirth { get; set; } = null!;
    }
}