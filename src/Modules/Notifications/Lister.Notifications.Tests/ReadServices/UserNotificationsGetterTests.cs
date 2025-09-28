using System.Text.Json;
using Lister.Notifications.Domain.ValueObjects;
using Lister.Notifications.Infrastructure.Sql;
using Lister.Notifications.Infrastructure.Sql.Entities;
using Lister.Notifications.Infrastructure.Sql.Services;
using Microsoft.EntityFrameworkCore;

namespace Lister.Notifications.Tests.ReadServices;

[TestFixture]
public class UserNotificationsGetterTests
{
    private static NotificationsDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<NotificationsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new NotificationsDbContext(options);
    }

    [Test]
    public async Task GetAsync_Filters_Maps_And_Paginates()
    {
        await using var db = CreateDb();
        var user = "u1";
        var listId = Guid.NewGuid();
        var content1 = new NotificationContent
            { Subject = "A", Body = "B", Data = new Dictionary<string, object> { ["k"] = 1 } };
        var content2 = new NotificationContent { Subject = "C", Body = "D" };
        db.Notifications.AddRange(
            new NotificationDb
            {
                Id = Guid.NewGuid(), UserId = user, ListId = listId, ContentJson = JsonSerializer.Serialize(content1),
                CreatedOn = DateTime.UtcNow.AddMinutes(-2)
            },
            new NotificationDb
            {
                Id = Guid.NewGuid(), UserId = user, ListId = listId, ContentJson = JsonSerializer.Serialize(content2),
                CreatedOn = DateTime.UtcNow.AddMinutes(-1), ReadOn = DateTime.UtcNow
            },
            new NotificationDb
            {
                Id = Guid.NewGuid(), UserId = "u2", ListId = listId, ContentJson = JsonSerializer.Serialize(content2),
                CreatedOn = DateTime.UtcNow
            }
        );
        await db.SaveChangesAsync();

        var getter = new UserNotificationsGetter(db);
        var since = DateTime.UtcNow.AddMinutes(-5);
        var page = await getter.GetAsync(user, since, listId, null, 10, 0, CancellationToken.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(page.TotalCount, Is.EqualTo(2));
            Assert.That(page.UnreadCount, Is.EqualTo(1));
            Assert.That(page.Notifications.Count, Is.EqualTo(2));
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(page.Notifications[1].Title, Is.EqualTo("A"));
            Assert.That(page.Notifications[1].Body, Is.EqualTo("B"));
            Assert.That(((Dictionary<string, object>)page.Notifications[1].Metadata!)["k"].ToString(), Is.EqualTo("1"));
        }
    }
}