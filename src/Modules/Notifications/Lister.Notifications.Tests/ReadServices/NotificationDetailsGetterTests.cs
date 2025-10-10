using System.Text.Json;
using Lister.Notifications.Domain.Enums;
using Lister.Notifications.Domain.ValueObjects;
using Lister.Notifications.Infrastructure.Sql;
using Lister.Notifications.Infrastructure.Sql.Entities;
using Lister.Notifications.Infrastructure.Sql.Services;
using Lister.Notifications.Infrastructure.Sql.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Lister.Notifications.Tests.ReadServices;

[TestFixture]
public class NotificationDetailsGetterTests
{
    private static NotificationsDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<NotificationsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new NotificationsDbContext(options);
    }

    [Test]
    public async Task GetAsync_Maps_Details_History_Attempts()
    {
        await using var db = CreateDb();
        var user = "u1";
        var listId = Guid.NewGuid();
        var id = Guid.NewGuid();
        var content = new NotificationContent
            { Subject = "Subject", Body = "Body", Data = new Dictionary<string, object> { ["x"] = 42 } };
        var n = new NotificationDb
        {
            Id = id,
            UserId = user,
            ListId = listId,
            ContentJson = JsonSerializer.Serialize(content),
            CreatedOn = DateTime.UtcNow
        };
        n.History.Add(new NotificationHistoryEntryDb
        {
            On = DateTime.UtcNow.AddMinutes(-1), By = user, Type = NotificationHistoryType.Created,
            Bag = new Dictionary<string, object?> { ["k"] = "v" }
        });
        n.DeliveryAttempts.Add(new NotificationDeliveryAttemptDb
        {
            AttemptedOn = DateTime.UtcNow,
            ChannelJson = JsonSerializer.Serialize(NotificationChannel.Email("user@example.com")),
            Status = (int)DeliveryStatus.Delivered,
            AttemptNumber = 1
        });
        db.Notifications.Add(n);
        await db.SaveChangesAsync();

        var getter = new NotificationDetailsGetter(db);
        var details = await getter.GetAsync(user, id, CancellationToken.None);

        Assert.That(details, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(details!.Id, Is.EqualTo(id));
            Assert.That(details.Title, Is.EqualTo("Subject"));
            Assert.That(details.Body, Is.EqualTo("Body"));
            Assert.That(((Dictionary<string, object>)details.Metadata!)["x"].ToString(), Is.EqualTo("42"));
            // Ensure our created history entry is present
            Assert.That(details.History.Any(h => h is { Type: NotificationHistoryType.Created, Bag: not null }
                                                 && ((Dictionary<string, object?>)h.Bag)["k"]?.ToString() == "v"),
                Is.True);
            Assert.That(details.DeliveryAttempts.Count, Is.EqualTo(1));
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(details.DeliveryAttempts[0].Channel, Is.EqualTo("Email"));
            Assert.That(details.DeliveryAttempts[0].Status, Is.EqualTo("Delivered"));
        }
    }
}