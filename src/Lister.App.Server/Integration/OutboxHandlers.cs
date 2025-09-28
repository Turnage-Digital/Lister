using System.Text.Json;
using Lister.Core.Domain.IntegrationEvents;
using Lister.Core.Infrastructure.Sql;
using Lister.Core.Infrastructure.Sql.Entities;
using MediatR;

namespace Lister.App.Server.Integration;

public class OutboxHandlerBase(CoreDbContext db)
{
    protected async Task EnqueueAsync<T>(T evt, CancellationToken ct)
    {
        var msg = new OutboxMessageDb
        {
            Type = typeof(T).FullName ?? typeof(T).Name,
            PayloadJson = JsonSerializer.Serialize(evt),
            CreatedOn = DateTime.UtcNow
        };
        db.OutboxMessages.Add(msg);
        await db.SaveChangesAsync(ct);
    }
}

public class ListItemCreatedOutboxHandler(CoreDbContext db)
    : OutboxHandlerBase(db), INotificationHandler<ListItemCreatedIntegrationEvent>
{
    public async Task Handle(ListItemCreatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        await EnqueueAsync(notification, cancellationToken);
    }
}

public class ListItemDeletedOutboxHandler(CoreDbContext db)
    : OutboxHandlerBase(db), INotificationHandler<ListItemDeletedIntegrationEvent>
{
    public async Task Handle(ListItemDeletedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        await EnqueueAsync(notification, cancellationToken);
    }
}

public class ListDeletedOutboxHandler(CoreDbContext db)
    : OutboxHandlerBase(db), INotificationHandler<ListDeletedIntegrationEvent>
{
    public async Task Handle(ListDeletedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        await EnqueueAsync(notification, cancellationToken);
    }
}

public class ListMigrationStartedOutboxHandler(CoreDbContext db)
    : OutboxHandlerBase(db), INotificationHandler<ListMigrationStartedIntegrationEvent>
{
    public async Task Handle(ListMigrationStartedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        await EnqueueAsync(notification, cancellationToken);
    }
}

public class ListMigrationProgressOutboxHandler(CoreDbContext db)
    : OutboxHandlerBase(db), INotificationHandler<ListMigrationProgressIntegrationEvent>
{
    public async Task Handle(ListMigrationProgressIntegrationEvent notification, CancellationToken cancellationToken)
    {
        await EnqueueAsync(notification, cancellationToken);
    }
}

public class ListMigrationCompletedOutboxHandler(CoreDbContext db)
    : OutboxHandlerBase(db), INotificationHandler<ListMigrationCompletedIntegrationEvent>
{
    public async Task Handle(ListMigrationCompletedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        await EnqueueAsync(notification, cancellationToken);
    }
}

public class ListMigrationFailedOutboxHandler(CoreDbContext db)
    : OutboxHandlerBase(db), INotificationHandler<ListMigrationFailedIntegrationEvent>
{
    public async Task Handle(ListMigrationFailedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        await EnqueueAsync(notification, cancellationToken);
    }
}

public class ListUpdatedOutboxHandler(CoreDbContext db)
    : OutboxHandlerBase(db), INotificationHandler<ListUpdatedIntegrationEvent>
{
    public async Task Handle(ListUpdatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        await EnqueueAsync(notification, cancellationToken);
    }
}