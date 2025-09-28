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
        using (Assert.EnterMultipleScope())
        {
            Assert.That(all.All(m => m.ProcessedOn != null), Is.True);
            Assert.That(all.All(m => m.Attempts == 1), Is.True);
        }
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
        using (Assert.EnterMultipleScope())
        {
            Assert.That(msg.ProcessedOn, Is.Null);
            Assert.That(msg.Attempts, Is.EqualTo(1));
            Assert.That(msg.LastError, Is.Not.Null);
        }
    }

    [Test]
    public async Task ProcessPendingOnce_SetsAvailableAfter_OnFailure_And_Skips_Until_Due()
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

        // First attempt fails and sets AvailableAfter
        await OutboxDispatcher.ProcessPendingOnceAsync(db, throwingFeed, logger, CancellationToken.None);
        var msg = db.OutboxMessages.Single();
        var firstAvailableAfter = msg.AvailableAfter;
        using (Assert.EnterMultipleScope())
        {
            Assert.That(firstAvailableAfter, Is.Not.Null);
            Assert.That(msg.Attempts, Is.EqualTo(1));
        }

        // Second pass should skip since AvailableAfter is in the future
        await OutboxDispatcher.ProcessPendingOnceAsync(db, throwingFeed, logger, CancellationToken.None);
        msg = db.OutboxMessages.Single();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(msg.Attempts, Is.EqualTo(1));
            Assert.That(msg.ProcessedOn, Is.Null);
        }
    }

    [Test]
    public async Task ProcessPendingOnce_CleansUp_Processed_Messages_Older_Than_Retention()
    {
        await using var db = CreateInMemoryDb();
        db.OutboxMessages.Add(new OutboxMessageDb
        {
            Type = "OldEvent",
            PayloadJson = "{}",
            CreatedOn = DateTime.UtcNow.AddDays(-10),
            ProcessedOn = DateTime.UtcNow.AddDays(-8),
            Attempts = 1
        });
        await db.SaveChangesAsync();

        var feed = new ChangeFeed();
        var logger = Mock.Of<ILogger<OutboxDispatcher>>();
        await OutboxDispatcher.ProcessPendingOnceAsync(db, feed, logger, CancellationToken.None);

        Assert.That(db.OutboxMessages.Count(), Is.EqualTo(0));
    }

    private class ThrowingFeed : ChangeFeed
    {
        public override ValueTask PublishAsync(object evt, CancellationToken cancellationToken = default)
        {
            throw new InvalidOperationException("boom");
        }
    }
}