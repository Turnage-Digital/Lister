using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using Lister.Lists.Domain;
using Lister.Lists.Domain.Enums;
using Lister.Lists.Domain.ValueObjects;
using Lister.Lists.Infrastructure.Sql.Entities;
using Lister.Notifications.Domain;
using Lister.Notifications.Domain.Enums;
using Lister.Notifications.Domain.ValueObjects;
using Lister.Notifications.Infrastructure.Sql.Entities;
using Lister.Users.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Serilog;

namespace Lister.App.Server;

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

            var services = scope.ServiceProvider;

            Log.Information("Seeding database...");

            var userManager = services.GetRequiredService<UserManager<User>>();
            var heath = EnsureUser(
                userManager,
                new UserSeed(
                    "heath@email.com",
                    "heath@email.com",
                    "Pass123$",
                    "Heath Turnage",
                    "Heath",
                    "Turnage",
                    "https://lister.com"
                )
            );
            var erika = EnsureUser(
                userManager,
                new UserSeed(
                    "erika@email.com",
                    "erika@email.com",
                    "Pass123$",
                    "Erika Turnage",
                    "Erika",
                    "Turnage",
                    "https://lister.com"
                )
            );

            var listAggregate = services.GetRequiredService<ListsAggregate<ListDb, ItemDb>>();
            var notifAggregate =
                services.GetRequiredService<NotificationAggregate<NotificationRuleDb, NotificationDb>>();

            EnsureStudentsList(listAggregate, notifAggregate, heath, erika);
            EnsureProjectsList(listAggregate, notifAggregate, heath, erika);
            EnsureInventoryList(listAggregate, notifAggregate, heath);

            Log.Information("Done seeding database. Exiting.");
        }
        catch (Exception e)
        {
            Log.Error(e, "Error seeding database");
        }
    }

    private static void EnsureStudentsList(
        ListsAggregate<ListDb, ItemDb> listAggregate,
        NotificationAggregate<NotificationRuleDb, NotificationDb> notifAggregate,
        User heath,
        User erika
    )
    {
        var existing = Await(listAggregate.GetListByNameAsync("Students"));
        if (existing is not null)
        {
            Log.Information("Students list already exists");
            return;
        }

        Status[] statuses =
        [
            CreateStatus("Active", "#FFCA28"),
            CreateStatus("Probation", "#FF7043"),
            CreateStatus("Inactive", "#607d8b"),
            CreateStatus("Graduated", "#66bb6a")
        ];

        Column[] columns =
        [
            TextColumn("Name", true, "name"),
            TextColumn("Address", storageKey: "address"),
            TextColumn("City", storageKey: "city"),
            TextColumn("State", storageKey: "state"),
            TextColumn("Zip Code", storageKey: "zipCode"),
            DateColumn("Date Of Birth", "dateOfBirth"),
            TextColumn("Guardian Name", storageKey: "guardianName"),
            TextColumn("Homeroom Teacher", storageKey: "homeroomTeacher"),
            NumberColumn("GPA", "gpa")
        ];

        StatusTransition[] transitions =
        [
            new() { From = "Active", AllowedNext = ["Probation", "Inactive", "Graduated"] },
            new() { From = "Probation", AllowedNext = ["Active", "Inactive"] },
            new() { From = "Inactive", AllowedNext = ["Active"] },
            new() { From = "Graduated", AllowedNext = [] }
        ];

        var list = Await(listAggregate.CreateListAsync(
            heath.Id,
            "Students",
            statuses,
            columns,
            transitions
        ));

        var statusNames = statuses.Select(s => s.Name).ToArray();
        var homeroomPrefixes = new[] { "Ms.", "Mr.", "Mx.", "Dr." };

        var faker = new Faker<Student>()
            .RuleFor(s => s.Name, f => f.Person.FullName)
            .RuleFor(s => s.Address, f => f.Address.StreetAddress())
            .RuleFor(s => s.City, f => f.Address.City())
            .RuleFor(s => s.State, f => f.Address.StateAbbr())
            .RuleFor(s => s.ZipCode, f => f.Address.ZipCode("#####"))
            .RuleFor(s => s.DateOfBirth, f => f.Date.Past(20, DateTime.UtcNow.AddYears(-16)).ToString("O"))
            .RuleFor(s => s.Status, f => f.PickRandom(statusNames))
            .RuleFor(s => s.GuardianName, f => $"{f.Name.FirstName()} {f.Name.LastName()}")
            .RuleFor(s => s.HomeroomTeacher,
                f => $"{f.PickRandom(homeroomPrefixes)} {f.Name.LastName()}")
            .RuleFor(s => s.Gpa, f => Math.Round(f.Random.Decimal(2.0m, 4.0m), 2));

        var students = faker.Generate(750);
        _ = Await(listAggregate.CreateItemsAsync(list, students, heath.Id));

        SeedStudentsNotifications(notifAggregate, list, heath, erika);
    }

    private static void SeedStudentsNotifications(
        NotificationAggregate<NotificationRuleDb, NotificationDb> notifAggregate,
        ListDb list,
        User heath,
        User erika
    )
    {
        var listId = RequireListId(list);

        var itemCreatedRule = Await(notifAggregate.CreateNotificationRuleAsync(
            heath.Id,
            listId,
            NotificationTrigger.ItemCreated(),
            [NotificationChannel.InApp(), NotificationChannel.Email(heath.Email!)],
            NotificationSchedule.Immediate(),
            null,
            true,
            CancellationToken.None
        ));

        var weeklyDigestRule = Await(notifAggregate.CreateNotificationRuleAsync(
            erika.Id,
            listId,
            NotificationTrigger.AnyStatusChange(),
            [NotificationChannel.Email(erika.Email!), NotificationChannel.InApp()],
            NotificationSchedule.Weekly(
                [DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Friday],
                new TimeOnly(9, 0)
            ),
            null,
            true,
            CancellationToken.None
        ));

        var delivered = Await(notifAggregate.CreateNotificationAsync(
            heath.Id,
            listId,
            null,
            itemCreatedRule.Id,
            new NotificationContent
            {
                Subject = "Welcome to Students",
                Body = "Sample delivered notification for the Students list.",
                Data = new Dictionary<string, object>
                {
                    ["seed"] = true,
                    ["type"] = "students",
                    ["itemsAdded"] = 3
                },
                ListName = list.Name,
                TriggeringUser = heath.Email,
                OccurredOn = DateTime.UtcNow.AddMinutes(-45)
            },
            NotificationPriority.Normal,
            CancellationToken.None
        ));
        Await(notifAggregate.RecordDeliveryAttemptAsync(
            delivered,
            NotificationChannel.InApp(),
            DeliveryStatus.Delivered,
            null,
            CancellationToken.None
        ));

        var failing = Await(notifAggregate.CreateNotificationAsync(
            heath.Id,
            listId,
            null,
            itemCreatedRule.Id,
            new NotificationContent
            {
                Subject = "Delivery failure example",
                Body = "This notification has a failed email attempt.",
                Data = new Dictionary<string, object> { ["seed"] = true },
                ListName = list.Name,
                TriggeringUser = heath.Email,
                OccurredOn = DateTime.UtcNow.AddMinutes(-15)
            },
            NotificationPriority.High,
            CancellationToken.None
        ));
        Await(notifAggregate.RecordDeliveryAttemptAsync(
            failing,
            NotificationChannel.Email(heath.Email!),
            DeliveryStatus.Failed,
            "Simulated SMTP error",
            CancellationToken.None
        ));

        var weeklyDigest = Await(notifAggregate.CreateNotificationAsync(
            erika.Id,
            listId,
            null,
            weeklyDigestRule.Id,
            new NotificationContent
            {
                Subject = "Weekly status digest",
                Body = "Summary of status changes for Students.",
                Data = new Dictionary<string, object>
                {
                    ["seed"] = true,
                    ["changes"] = new[]
                    {
                        new { student = "Jordan Owens", from = "Probation", to = "Active" },
                        new { student = "Riley Mills", from = "Active", to = "Graduated" },
                        new { student = "Kai Bishop", from = "Active", to = "Probation" }
                    }
                },
                ListName = list.Name,
                TriggeringUser = erika.Email,
                OccurredOn = DateTime.UtcNow.AddDays(-1)
            },
            NotificationPriority.Low,
            CancellationToken.None
        ));
        Await(notifAggregate.RecordDeliveryAttemptAsync(
            weeklyDigest,
            NotificationChannel.Email(erika.Email!),
            DeliveryStatus.Delivered,
            null,
            CancellationToken.None
        ));
        Await(notifAggregate.MarkNotificationAsReadAsync(
            weeklyDigest,
            DateTime.UtcNow.AddHours(-18),
            CancellationToken.None
        ));

        var erikaUnread = Await(notifAggregate.CreateNotificationAsync(
            erika.Id,
            listId,
            null,
            weeklyDigestRule.Id,
            new NotificationContent
            {
                Subject = "Erika - Unread",
                Body = "Unread sample",
                ListName = list.Name,
                TriggeringUser = heath.Email,
                OccurredOn = DateTime.UtcNow.AddMinutes(-5)
            },
            NotificationPriority.Low,
            CancellationToken.None
        ));

        var erikaRead = Await(notifAggregate.CreateNotificationAsync(
            erika.Id,
            listId,
            null,
            weeklyDigestRule.Id,
            new NotificationContent
            {
                Subject = "Erika - Read",
                Body = "Read sample",
                ListName = list.Name,
                TriggeringUser = heath.Email,
                OccurredOn = DateTime.UtcNow.AddMinutes(-30)
            },
            NotificationPriority.Low,
            CancellationToken.None
        ));
        Await(notifAggregate.RecordDeliveryAttemptAsync(
            erikaRead,
            NotificationChannel.InApp(),
            DeliveryStatus.Delivered,
            null,
            CancellationToken.None
        ));
        Await(notifAggregate.MarkNotificationAsReadAsync(
            erikaRead,
            DateTime.UtcNow.AddMinutes(-10),
            CancellationToken.None
        ));
    }

    private static void EnsureProjectsList(
        ListsAggregate<ListDb, ItemDb> listAggregate,
        NotificationAggregate<NotificationRuleDb, NotificationDb> notifAggregate,
        User heath,
        User erika
    )
    {
        var existing = Await(listAggregate.GetListByNameAsync("Projects"));
        if (existing is not null)
        {
            Log.Information("Projects list already exists");
            return;
        }

        Status[] statuses =
        [
            CreateStatus("Backlog", "#90a4ae"),
            CreateStatus("In Progress", "#42a5f5"),
            CreateStatus("Blocked", "#ef5350"),
            CreateStatus("Done", "#66bb6a")
        ];

        Column[] columns =
        [
            TextColumn("Title", true, "title"),
            TextColumn("Summary", storageKey: "summary"),
            TextColumn("Owner", storageKey: "owner"),
            DateColumn("Due Date", "dueDate"),
            NumberColumn("Priority", "priority"),
            NumberColumn("Budget", "budget"),
            TextColumn("Tags", storageKey: "tags")
        ];

        StatusTransition[] transitions =
        [
            new() { From = "Backlog", AllowedNext = ["In Progress", "Blocked"] },
            new() { From = "In Progress", AllowedNext = ["Blocked", "Done"] },
            new() { From = "Blocked", AllowedNext = ["In Progress"] },
            new() { From = "Done", AllowedNext = [] }
        ];

        var list = Await(listAggregate.CreateListAsync(
            heath.Id,
            "Projects",
            statuses,
            columns,
            transitions
        ));

        var statusNames = statuses.Select(s => s.Name).ToArray();
        var tagsPool = new[] { "api", "mobile", "web", "backend", "infrastructure", "ux", "research", "ops" };
        var owners = new[] { "Heath Turnage", "Erika Turnage", "Sam Price", "Lydia Hart", "Noah Gray" };

        var faker = new Faker<Project>()
            .RuleFor(p => p.Title, f => f.Company.CatchPhrase())
            .RuleFor(p => p.Summary, f => f.Lorem.Sentence(15))
            .RuleFor(p => p.Owner, f => f.PickRandom(owners))
            .RuleFor(p => p.DueDate, f => f.Date.Soon(180).ToString("O"))
            .RuleFor(p => p.Priority, f => f.Random.Int(1, 5))
            .RuleFor(p => p.Budget, f => Math.Round(f.Random.Decimal(10_000m, 250_000m), 2))
            .RuleFor(p => p.Tags, f => string.Join(", ", f.Random.ArrayElements(tagsPool, f.Random.Int(1, 3))))
            .RuleFor(p => p.Status, f => f.PickRandom(statusNames));

        var projects = faker.Generate(275);
        _ = Await(listAggregate.CreateItemsAsync(list, projects, heath.Id));

        SeedProjectsNotifications(notifAggregate, list, heath, erika);
    }

    private static void SeedProjectsNotifications(
        NotificationAggregate<NotificationRuleDb, NotificationDb> notifAggregate,
        ListDb list,
        User heath,
        User erika
    )
    {
        var listId = RequireListId(list);

        var completionRule = Await(notifAggregate.CreateNotificationRuleAsync(
            erika.Id,
            listId,
            NotificationTrigger.StatusChanged("In Progress", "Done"),
            [NotificationChannel.Email(erika.Email!), NotificationChannel.InApp()],
            NotificationSchedule.Delayed(TimeSpan.FromMinutes(10)),
            null,
            true,
            CancellationToken.None
        ));

        var deletionRule = Await(notifAggregate.CreateNotificationRuleAsync(
            heath.Id,
            listId,
            NotificationTrigger.ItemDeleted(),
            [NotificationChannel.Webhook("https://hooks.example.com/projects"), NotificationChannel.InApp()],
            NotificationSchedule.Immediate(),
            null,
            true,
            CancellationToken.None
        ));

        var completion = Await(notifAggregate.CreateNotificationAsync(
            erika.Id,
            listId,
            null,
            completionRule.Id,
            new NotificationContent
            {
                Subject = "Project shipped!",
                Body = "Project Surge completed and ready for launch.",
                ListName = list.Name,
                ItemIdentifier = "PRJ-204",
                OldValue = "In Progress",
                NewValue = "Done",
                TriggeringUser = heath.Email,
                OccurredOn = DateTime.UtcNow.AddMinutes(-30),
                Data = new Dictionary<string, object>
                {
                    ["seed"] = true,
                    ["priority"] = "High",
                    ["contributors"] = new[] { "Heath", "Erika", "Sam" }
                }
            },
            NotificationPriority.High,
            CancellationToken.None
        ));
        Await(notifAggregate.RecordDeliveryAttemptAsync(
            completion,
            NotificationChannel.Email(erika.Email!),
            DeliveryStatus.Delivered,
            null,
            CancellationToken.None
        ));
        Await(notifAggregate.RecordDeliveryAttemptAsync(
            completion,
            NotificationChannel.InApp(),
            DeliveryStatus.Delivered,
            null,
            CancellationToken.None
        ));
        Await(notifAggregate.MarkNotificationAsReadAsync(
            completion,
            DateTime.UtcNow.AddMinutes(-5),
            CancellationToken.None
        ));

        var webhookChannel = NotificationChannel.Webhook("https://hooks.example.com/projects");
        var webhookFailure = Await(notifAggregate.CreateNotificationAsync(
            heath.Id,
            listId,
            null,
            deletionRule.Id,
            new NotificationContent
            {
                Subject = "Webhook failure sample",
                Body = "Example payload for failed webhook retry.",
                ListName = list.Name,
                TriggeringUser = "System",
                OccurredOn = DateTime.UtcNow.AddMinutes(-2),
                Data = new Dictionary<string, object>
                {
                    ["seed"] = true,
                    ["action"] = "ItemDeleted"
                }
            },
            NotificationPriority.Normal,
            CancellationToken.None
        ));
        Await(notifAggregate.RecordDeliveryAttemptAsync(
            webhookFailure,
            webhookChannel,
            DeliveryStatus.Failed,
            "Simulated 503 from webhook endpoint",
            CancellationToken.None
        ));
        Await(notifAggregate.RecordDeliveryAttemptAsync(
            webhookFailure,
            webhookChannel,
            DeliveryStatus.Retry,
            "Retry scheduled in 15 minutes",
            CancellationToken.None
        ));
    }

    private static void EnsureInventoryList(
        ListsAggregate<ListDb, ItemDb> listAggregate,
        NotificationAggregate<NotificationRuleDb, NotificationDb> notifAggregate,
        User heath
    )
    {
        var existing = Await(listAggregate.GetListByNameAsync("Inventory"));
        if (existing is not null)
        {
            Log.Information("Inventory list already exists");
            return;
        }

        Status[] statuses =
        [
            CreateStatus("Available", "#8bc34a"),
            CreateStatus("In Use", "#29b6f6"),
            CreateStatus("Maintenance", "#ffc107"),
            CreateStatus("Retired", "#bdbdbd")
        ];

        Column[] columns =
        [
            TextColumn("Asset Tag", true, "assetTag"),
            TextColumn("Category", true, "category"),
            TextColumn("Location", storageKey: "location"),
            TextColumn("Assigned To", storageKey: "assignedTo"),
            DateColumn("Purchase Date", "purchaseDate"),
            DateColumn("Warranty Expiration", "warrantyExpiration"),
            NumberColumn("Quantity", "quantity"),
            NumberColumn("Cost", "cost"),
            TextColumn("Vendor", storageKey: "vendor"),
            TextColumn("Notes", storageKey: "notes")
        ];

        StatusTransition[] transitions =
        [
            new() { From = "Available", AllowedNext = ["In Use", "Maintenance", "Retired"] },
            new() { From = "In Use", AllowedNext = ["Maintenance", "Retired"] },
            new() { From = "Maintenance", AllowedNext = ["Available", "Retired"] },
            new() { From = "Retired", AllowedNext = [] }
        ];

        var list = Await(listAggregate.CreateListAsync(
            heath.Id,
            "Inventory",
            statuses,
            columns,
            transitions
        ));

        var statusNames = statuses.Select(s => s.Name).ToArray();
        var categories = new[] { "Audio", "Video", "Networking", "Office", "Manufacturing", "Safety", "Vehicles" };

        var faker = new Faker<InventoryItem>()
            .RuleFor(i => i.AssetTag, f => $"AST-{f.Random.Int(1000, 9999)}")
            .RuleFor(i => i.Category, f => f.PickRandom(categories))
            .RuleFor(i => i.Location, f => $"{f.Address.City()}, {f.Address.StateAbbr()}")
            .RuleFor(i => i.AssignedTo,
                f => f.Random.Bool(0.6f) ? $"{f.Name.FirstName()} {f.Name.LastName()}" : "Unassigned")
            .RuleFor(i => i.PurchaseDate, f => f.Date.Past(5).ToString("O"))
            .RuleFor(i => i.WarrantyExpiration, f => f.Date.Future(2).ToString("O"))
            .RuleFor(i => i.Quantity, f => f.Random.Int(1, 50))
            .RuleFor(i => i.Cost, f => Math.Round(f.Random.Decimal(50m, 5000m), 2))
            .RuleFor(i => i.Vendor, f => f.Company.CompanyName())
            .RuleFor(i => i.Notes, f => f.Random.Bool(0.4f) ? f.Lorem.Sentence(12) : string.Empty)
            .RuleFor(i => i.Status, f => f.PickRandom(statusNames));

        var items = faker.Generate(300);
        _ = Await(listAggregate.CreateItemsAsync(list, items, heath.Id));

        SeedInventoryNotifications(notifAggregate, list, heath);
    }

    private static void SeedInventoryNotifications(
        NotificationAggregate<NotificationRuleDb, NotificationDb> notifAggregate,
        ListDb list,
        User heath
    )
    {
        var listId = RequireListId(list);

        var maintenanceRule = Await(notifAggregate.CreateNotificationRuleAsync(
            heath.Id,
            listId,
            NotificationTrigger.StatusChanged("In Use", "Maintenance"),
            [NotificationChannel.Email(heath.Email!), NotificationChannel.Sms("+15555551515")],
            NotificationSchedule.Immediate(),
            null,
            true,
            CancellationToken.None
        ));

        var maintenanceAlert = Await(notifAggregate.CreateNotificationAsync(
            heath.Id,
            listId,
            null,
            maintenanceRule.Id,
            new NotificationContent
            {
                Subject = "Maintenance required",
                Body = "Forklift FL-443 scheduled for maintenance.",
                ListName = list.Name,
                ItemIdentifier = "AST-443",
                OldValue = "In Use",
                NewValue = "Maintenance",
                TriggeringUser = "Maintenance Bot",
                OccurredOn = DateTime.UtcNow.AddHours(-6),
                Data = new Dictionary<string, object>
                {
                    ["seed"] = true,
                    ["assetTag"] = "AST-443",
                    ["location"] = "Warehouse A"
                }
            },
            NotificationPriority.Critical,
            CancellationToken.None
        ));

        Await(notifAggregate.RecordDeliveryAttemptAsync(
            maintenanceAlert,
            NotificationChannel.Sms("+15555551515"),
            DeliveryStatus.Queued,
            null,
            CancellationToken.None
        ));
        Await(notifAggregate.RecordDeliveryAttemptAsync(
            maintenanceAlert,
            NotificationChannel.Email(heath.Email!),
            DeliveryStatus.Delivered,
            null,
            CancellationToken.None
        ));
    }

    private static User EnsureUser(UserManager<User> userManager, UserSeed seed)
    {
        var existing = Await(userManager.FindByNameAsync(seed.UserName));
        if (existing is not null)
        {
            Log.Information("{UserName} already exists", seed.UserName);
            return existing;
        }

        var created = CreateUser(userManager, seed);
        Log.Information("Created seed user {UserName}", seed.UserName);
        return created;
    }

    private static User CreateUser(UserManager<User> userManager, UserSeed seed)
    {
        var user = new User
        {
            UserName = seed.UserName,
            Email = seed.Email,
            EmailConfirmed = true
        };

        var createResult = Await(userManager.CreateAsync(user, seed.Password));
        if (!createResult.Succeeded)
        {
            throw new Exception(createResult.Errors.First().Description);
        }

        var claimsResult = Await(userManager.AddClaimsAsync(user, new[]
        {
            new Claim(JwtClaimTypes.Name, seed.Name),
            new Claim(JwtClaimTypes.GivenName, seed.GivenName),
            new Claim(JwtClaimTypes.FamilyName, seed.FamilyName),
            new Claim(JwtClaimTypes.WebSite, seed.Website)
        }));

        if (!claimsResult.Succeeded)
        {
            throw new Exception(claimsResult.Errors.First().Description);
        }

        return user;
    }

    private static Guid RequireListId(ListDb list)
    {
        if (list.Id is null)
        {
            throw new InvalidOperationException("List must have an Id before seeding items.");
        }

        return list.Id.Value;
    }

    private static Status CreateStatus(string name, string color)
    {
        return new Status
        {
            Name = name,
            Color = color
        };
    }

    private static Column TextColumn(string name, bool required = false, string? storageKey = null)
    {
        return new Column
        {
            StorageKey = storageKey,
            Name = name,
            Type = ColumnType.Text,
            Required = required
        };
    }

    private static Column DateColumn(string name, string? storageKey = null)
    {
        return new Column
        {
            StorageKey = storageKey,
            Name = name,
            Type = ColumnType.Date
        };
    }

    private static Column NumberColumn(string name, string? storageKey = null)
    {
        return new Column
        {
            StorageKey = storageKey,
            Name = name,
            Type = ColumnType.Number
        };
    }

    private static T Await<T>(Task<T> task)
    {
        return task.GetAwaiter().GetResult();
    }

    private static void Await(Task task)
    {
        task.GetAwaiter().GetResult();
    }

    private static class JwtClaimTypes
    {
        public const string Name = "name";
        public const string GivenName = "given_name";
        public const string FamilyName = "family_name";
        public const string WebSite = "website";
    }

    private sealed record UserSeed(
        string UserName,
        string Email,
        string Password,
        string Name,
        string GivenName,
        string FamilyName,
        string Website
    );

    private sealed class Student
    {
        public string City { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public string DateOfBirth { get; set; } = string.Empty;
        public string GuardianName { get; set; } = string.Empty;
        public string HomeroomTeacher { get; set; } = string.Empty;
        public decimal Gpa { get; set; }
    }

    private sealed class Project
    {
        public string Title { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public string Owner { get; set; } = string.Empty;
        public string DueDate { get; set; } = string.Empty;
        public int Priority { get; set; }
        public decimal Budget { get; set; }
        public string Tags { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    private sealed class InventoryItem
    {
        public string AssetTag { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string AssignedTo { get; set; } = string.Empty;
        public string PurchaseDate { get; set; } = string.Empty;
        public string WarrantyExpiration { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Cost { get; set; }
        public string Vendor { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
#endif
