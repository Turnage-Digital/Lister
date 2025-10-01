using System.Text.Json;
using Lister.Core.Domain.IntegrationEvents;
using Lister.Core.Infrastructure.Sql;
using Lister.Core.Infrastructure.Sql.Entities;
using MediatR;

namespace Lister.App.Server.Integration;

public class OutboxHandlerBase(CoreDbContext dbContext)
{
    protected async Task EnqueueAsync<T>(T @event, CancellationToken cancellationToken)
    {
        var msg = new OutboxMessageDb
        {
            Type = typeof(T).FullName ?? typeof(T).Name,
            PayloadJson = JsonSerializer.Serialize(@event),
            CreatedOn = DateTime.UtcNow
        };
        dbContext.OutboxMessages.Add(msg);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}

public class ListItemCreatedOutboxHandler(CoreDbContext dbContext)
    : OutboxHandlerBase(dbContext), INotificationHandler<ListItemCreatedIntegrationEvent>
{
    public async Task Handle(ListItemCreatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        await EnqueueAsync(notification, cancellationToken);
    }
}

public class ListItemDeletedOutboxHandler(CoreDbContext dbContext)
    : OutboxHandlerBase(dbContext), INotificationHandler<ListItemDeletedIntegrationEvent>
{
    public async Task Handle(ListItemDeletedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        await EnqueueAsync(notification, cancellationToken);
    }
}

public class ListDeletedOutboxHandler(CoreDbContext dbContext)
    : OutboxHandlerBase(dbContext), INotificationHandler<ListDeletedIntegrationEvent>
{
    public async Task Handle(ListDeletedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        await EnqueueAsync(notification, cancellationToken);
    }
}

public class ListMigrationStartedOutboxHandler(CoreDbContext dbContext)
    : OutboxHandlerBase(dbContext), INotificationHandler<ListMigrationStartedIntegrationEvent>
{
    public async Task Handle(ListMigrationStartedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        await EnqueueAsync(notification, cancellationToken);
    }
}

public class ListMigrationProgressOutboxHandler(CoreDbContext dbContext)
    : OutboxHandlerBase(dbContext), INotificationHandler<ListMigrationProgressIntegrationEvent>
{
    public async Task Handle(ListMigrationProgressIntegrationEvent notification, CancellationToken cancellationToken)
    {
        await EnqueueAsync(notification, cancellationToken);
    }
}

public class ListMigrationCompletedOutboxHandler(CoreDbContext dbContext)
    : OutboxHandlerBase(dbContext), INotificationHandler<ListMigrationCompletedIntegrationEvent>
{
    public async Task Handle(ListMigrationCompletedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        await EnqueueAsync(notification, cancellationToken);
    }
}

public class ListMigrationFailedOutboxHandler(CoreDbContext dbContext)
    : OutboxHandlerBase(dbContext), INotificationHandler<ListMigrationFailedIntegrationEvent>
{
    public async Task Handle(ListMigrationFailedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        await EnqueueAsync(notification, cancellationToken);
    }
}

public class ListUpdatedOutboxHandler(CoreDbContext dbContext)
    : OutboxHandlerBase(dbContext), INotificationHandler<ListUpdatedIntegrationEvent>
{
    public async Task Handle(ListUpdatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        await EnqueueAsync(notification, cancellationToken);
    }
}