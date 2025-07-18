using System.Security.Claims;
using Bogus;
using Lister.Lists.Domain;
using Lister.Lists.Domain.Enums;
using Lister.Lists.Domain.ValueObjects;
using Lister.Lists.Infrastructure.Sql.Entities;
using Lister.Users.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Serilog;

namespace Lister.Server;

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

            Log.Information("Seeding database...");

            var userManager = scope.ServiceProvider
                .GetRequiredService<UserManager<User>>();

            var heath = userManager.FindByNameAsync("heath@email.com").Result;
            if (heath == null)
            {
                CreateUser(userManager, "heath@email.com", "heath@email.com", "Pass123$", "Heath Turnage",
                    "Heath", "Turnage", "https://lister.com");
                heath = userManager.FindByNameAsync("heath@email.com").Result;
            }
            else
            {
                Log.Information("Heath already exists");
            }

            var erika = userManager.FindByNameAsync("erika@email.com").Result;
            if (erika == null)
            {
                CreateUser(userManager, "erika@email.com", "erika@email.com", "Pass123$", "Erika Turnage",
                    "Erika", "Turnage", "https://lister.com");
            }
            else
            {
                Log.Information("Erika already exists");
            }

            var listAggregate = scope.ServiceProvider
                .GetRequiredService<ListsAggregate<ListDb, ItemDb>>();

            var list = listAggregate.GetListByNameAsync("Students").Result;
            if (list == null)
            {
                list = listAggregate.CreateListAsync(
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
                    )
                    .Result;

                var faker = new Faker<Student>()
                    .RuleFor(s => s.City, f => f.Person.Address.City)
                    .RuleFor(s => s.Name, f => f.Person.FullName)
                    .RuleFor(s => s.State, f => f.Person.Address.State)
                    .RuleFor(s => s.Status, f => f.PickRandom("Active", "Inactive"))
                    .RuleFor(s => s.Address, f => f.Person.Address.Street)
                    .RuleFor(s => s.ZipCode, f => f.Address.ZipCode())
                    .RuleFor(s => s.DateOfBirth, f => f.Person.DateOfBirth.Date.ToString("O"));
                var students = faker.Generate(500);

                listAggregate.CreateItemsAsync(list, students, heath.Id).Wait();
            }

            Log.Information("Done seeding database. Exiting.");
        }
        catch (Exception e)
        {
            Log.Error(e, "Error seeding database");
        }
    }

    private static void CreateUser(
        UserManager<User> userManager,
        string userName,
        string email,
        string password,
        string name,
        string givenName,
        string familyName,
        string website
    )
    {
        var user = new User
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

        result = userManager.AddClaimsAsync(user, [
                new Claim(JwtClaimTypes.Name, name),
                new Claim(JwtClaimTypes.GivenName, givenName),
                new Claim(JwtClaimTypes.FamilyName, familyName),
                new Claim(JwtClaimTypes.WebSite, website)
            ])
            .Result;

        if (!result.Succeeded)
        {
            throw new Exception(result.Errors.First().Description);
        }

        Console.WriteLine($"{userName} created");
    }

    public static class JwtClaimTypes
    {
        public const string Name = "name";
        public const string GivenName = "given_name";
        public const string FamilyName = "family_name";
        public const string WebSite = "website";
    }

    public class Student
    {
        public string City { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string State { get; set; } = null!;
        public string Status { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string ZipCode { get; set; } = null!;
        public string DateOfBirth { get; set; } = null!;
    }
}
#endif