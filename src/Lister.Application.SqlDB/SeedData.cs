using System.Security.Claims;
using Bogus;
using Lister.Core.Enums;
using Lister.Core.SqlDB.Entities;
using Lister.Core.ValueObjects;
using Lister.Domain;
using Microsoft.AspNetCore.Identity;

namespace Lister.Application.SqlDB;

#if DEBUG
public static class SeedData
{
    public static void EnsureSeedData(WebApplication app)
    {
        try
        {
            using var scope = app.Services
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope();

            Console.WriteLine("Seeding database...");

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
                Console.WriteLine("heath already exists");
            }

            var erika = userManager.FindByNameAsync("erika").Result;
            if (erika == null)
            {
                CreateUser(userManager, "erika", "erika@email.com", "Pass123$", "Erika Turnage",
                    "Erika", "Turnage", "https://lister.com");
            }
            else
            {
                Console.WriteLine("erika already exists");
            }

            var listAggregate = scope.ServiceProvider
                .GetRequiredService<ListAggregate<ListEntity, ItemEntity>>();

            var list = listAggregate.FindByNameAsync("Students").Result;
            if (list == null)
            {
                list = listAggregate.CreateAsync(
                    heath!.Id,
                    "Students",
                    [
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
                    ],
                    [
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
                    ]
                ).Result;

                var faker = new Faker<Student>()
                    .RuleFor(s => s.City, f => f.Person.Address.City)
                    .RuleFor(s => s.Name, f => f.Person.FullName)
                    .RuleFor(s => s.State, f => f.Person.Address.State)
                    .RuleFor(s => s.Status, f => f.PickRandom("Active", "Inactive"))
                    .RuleFor(s => s.Address, f => f.Person.Address.Street)
                    .RuleFor(s => s.ZipCode, f => f.Address.ZipCode())
                    .RuleFor(s => s.DateOfBirth, f => f.Person.DateOfBirth.Date.ToString("O"));
                var students = faker.Generate(100000);

                listAggregate.AddListItemsAsync(list, heath.Id, students).Wait();
            }

            Console.WriteLine("Done seeding database. Exiting.");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
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

        result = userManager.AddClaimsAsync(user, [
            new Claim(JwtClaimTypes.Name, name),
            new Claim(JwtClaimTypes.GivenName, givenName),
            new Claim(JwtClaimTypes.FamilyName, familyName),
            new Claim(JwtClaimTypes.WebSite, website)
        ]).Result;

        if (!result.Succeeded)
        {
            throw new Exception(result.Errors.First().Description);
        }

        Console.WriteLine($"{userName} created");
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
        public string City { get; } = null!;
        public string Name { get; } = null!;
        public string State { get; } = null!;
        public string Status { get; } = null!;
        public string Address { get; } = null!;
        public string ZipCode { get; } = null!;
        public string DateOfBirth { get; } = null!;
    }
}
#endif