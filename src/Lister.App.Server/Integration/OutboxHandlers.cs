using System.Text.Json;
using Lister.Core.Domain.IntegrationEvents;
using Lister.Core.Infrastructure.Sql.Outbox;
using MediatR;

namespace Lister.App.Server.Integration;

public class OutboxHandlerBase(OutboxDbContext db)
{
    protected async Task EnqueueAsync<T>(T evt, CancellationToken ct)
    {
        var msg = new OutboxMessageDb
        {
            Type = typeof(T).FullName ?? typeof(T).Name,
            PayloadJson = JsonSerializer.Serialize(evt),
            CreatedOn = DateTime.UtcNow
        };
        db.Messages.Add(msg);
        await db.SaveChangesAsync(ct);
    }
}

public class ListItemCreatedOutboxHandler(OutboxDbContext db)
    : OutboxHandlerBase(db), INotificationHandler<ListItemCreatedIntegrationEvent>
{
    public async Task Handle(ListItemCreatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        await EnqueueAsync(notification, cancellationToken);
    }
}

public class ListItemDeletedOutboxHandler(OutboxDbContext db)
    : OutboxHandlerBase(db), INotificationHandler<ListItemDeletedIntegrationEvent>
{
    public async Task Handle(ListItemDeletedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        await EnqueueAsync(notification, cancellationToken);
    }
}

public class ListDeletedOutboxHandler(OutboxDbContext db)
    : OutboxHandlerBase(db), INotificationHandler<ListDeletedIntegrationEvent>
{
    public async Task Handle(ListDeletedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        await EnqueueAsync(notification, cancellationToken);
    }
}