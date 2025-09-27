using Lister.App.Server.Integration;
using Lister.Core.Domain.IntegrationEvents;
using Lister.Core.Infrastructure.Sql;
using Lister.Notifications.Domain.Entities;
using Lister.Notifications.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace Lister.App.Server.Tests;

[TestFixture]
public class OutboxHandlersTests
{
    private static CoreDbContext CreateInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<CoreDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new CoreDbContext(options);
    }

    [Test]
    public async Task ListItemCreated_Handler_Enqueues_Outbox_Message()
    {
        await using var db = CreateInMemoryDb();
        var handler = new ListItemCreatedOutboxHandler(db);
        var evt = new ListItemCreatedIntegrationEvent(Guid.NewGuid(), 123, "tester");

        await handler.Handle(evt, CancellationToken.None);

        var msg = db.OutboxMessages.Single();
        Assert.That(msg.Type, Is.EqualTo(typeof(ListItemCreatedIntegrationEvent).FullName));
        Assert.That(msg.PayloadJson, Does.Contain("\"EventType\":\"ListItemCreatedIntegrationEvent\""));
        Assert.That(msg.Attempts, Is.EqualTo(0));
        Assert.That(msg.ProcessedOn, Is.Null);
    }

    private class FakeNotification : IWritableNotification
    {
        public Guid? Id { get; } = Guid.NewGuid();
        public Guid? NotificationRuleId { get; set; }
        public string UserId { get; } = "u";
        public Guid ListId { get; } = Guid.NewGuid();
        public int? ItemId { get; } = 1;
    }

    [Test]
    public async Task NotificationCreated_Handler_Enqueues_Outbox_Message()
    {
        await using var db = CreateInMemoryDb();
        var handler = new NotificationCreatedOutboxHandler(db);
        var evt = new NotificationCreatedEvent(new FakeNotification(), "tester");

        await handler.Handle(evt, CancellationToken.None);

        var msg = db.OutboxMessages.Single();
        Assert.That(msg.Type, Is.EqualTo(typeof(NotificationCreatedEvent).FullName));
        Assert.That(msg.PayloadJson, Does.Contain("\"CreatedBy\":\"tester\""));
    }
}