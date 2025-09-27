using Lister.Notifications.Infrastructure.Sql;
using Lister.Notifications.Infrastructure.Sql.Entities;
using Lister.Notifications.Infrastructure.Sql.Services;
using Microsoft.EntityFrameworkCore;

namespace Lister.Notifications.Tests.ReadServices;

[TestFixture]
public class UnreadNotificationCountGetterTests
{
    private static NotificationsDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<NotificationsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new NotificationsDbContext(options);
    }

    [Test]
    public async Task GetAsync_Counts_Unread_Optionally_By_List()
    {
        await using var db = CreateDb();
        var user = "u1";
        var listA = Guid.NewGuid();
        var listB = Guid.NewGuid();

        var empty = "{}";
        db.Notifications.AddRange(
            new NotificationDb
            {
                Id = Guid.NewGuid(), UserId = user, ListId = listA, ContentJson = empty, CreatedOn = DateTime.UtcNow
            },
            new NotificationDb
            {
                Id = Guid.NewGuid(), UserId = user, ListId = listA, ContentJson = empty, CreatedOn = DateTime.UtcNow,
                ReadOn = DateTime.UtcNow
            },
            new NotificationDb
            {
                Id = Guid.NewGuid(), UserId = user, ListId = listB, ContentJson = empty, CreatedOn = DateTime.UtcNow
            },
            new NotificationDb
                { Id = Guid.NewGuid(), UserId = "u2", ListId = listA, ContentJson = empty, CreatedOn = DateTime.UtcNow }
        );
        await db.SaveChangesAsync();

        var getter = new UnreadNotificationCountGetter(db);
        var total = await getter.GetAsync(user, null, CancellationToken.None);
        var listACount = await getter.GetAsync(user, listA, CancellationToken.None);

        Assert.That(total, Is.EqualTo(2));
        Assert.That(listACount, Is.EqualTo(1));
    }
}