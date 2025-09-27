using Lister.App.Server.Services;
using Lister.Core.Infrastructure.Sql;
using Lister.Core.Infrastructure.Sql.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace Lister.App.Server.Tests;

[TestFixture]
public class OutboxDispatcherTests
{
    private static CoreDbContext CreateInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<CoreDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new CoreDbContext(options);
    }

    [Test]
    public async Task ProcessPendingOnce_MarksProcessed_OnSuccess()
    {
        await using var db = CreateInMemoryDb();
        db.OutboxMessages.Add(new OutboxMessageDb
        {
            Type = "TestEvent",
            PayloadJson = "{\"a\":1}",
            CreatedOn = DateTime.UtcNow.AddMinutes(-1)
        });
        db.OutboxMessages.Add(new OutboxMessageDb
        {
            Type = "TestEvent2",
            PayloadJson = "{\"b\":2}",
            CreatedOn = DateTime.UtcNow
        });
        await db.SaveChangesAsync();

        var feed = new ChangeFeed();
        var logger = Mock.Of<ILogger<OutboxDispatcher>>();

        await OutboxDispatcher.ProcessPendingOnceAsync(db, feed, logger, CancellationToken.None);

        var all = db.OutboxMessages.ToList();
        Assert.That(all.All(m => m.ProcessedOn != null), Is.True);
        Assert.That(all.All(m => m.Attempts == 1), Is.True);
    }

    [Test]
    public async Task ProcessPendingOnce_IncrementsAttempts_And_SetsError_OnFailure()
    {
        await using var db = CreateInMemoryDb();
        db.OutboxMessages.Add(new OutboxMessageDb
        {
            Type = "BadEvent",
            PayloadJson = "{bad json}",
            CreatedOn = DateTime.UtcNow
        });
        await db.SaveChangesAsync();

        var throwingFeed = new ThrowingFeed();
        var logger = Mock.Of<ILogger<OutboxDispatcher>>();

        await OutboxDispatcher.ProcessPendingOnceAsync(db, throwingFeed, logger, CancellationToken.None);

        var msg = db.OutboxMessages.Single();
        Assert.That(msg.ProcessedOn, Is.Null);
        Assert.That(msg.Attempts, Is.EqualTo(1));
        Assert.That(msg.LastError, Is.Not.Null);
    }

    private class ThrowingFeed : ChangeFeed
    {
        public override ValueTask PublishAsync(object evt, CancellationToken cancellationToken = default)
        {
            throw new InvalidOperationException("boom");
        }
    }
}
