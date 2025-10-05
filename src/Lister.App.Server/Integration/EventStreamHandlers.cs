using Lister.App.Server.Services;
using Lister.Core.Domain.IntegrationEvents;
using MediatR;

namespace Lister.App.Server.Integration;

public class ListItemCreatedStreamHandler(ChangeFeed feed) : INotificationHandler<ListItemCreatedIntegrationEvent>
{
    public async Task Handle(ListItemCreatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var envelope = new
            { type = nameof(ListItemCreatedIntegrationEvent), data = notification, occurredOn = DateTime.UtcNow };
        await feed.PublishAsync(envelope, cancellationToken);
    }
}

public class ListItemUpdatedStreamHandler(ChangeFeed feed) : INotificationHandler<ListItemUpdatedIntegrationEvent>
{
    public async Task Handle(ListItemUpdatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var envelope = new
            { type = nameof(ListItemUpdatedIntegrationEvent), data = notification, occurredOn = DateTime.UtcNow };
        await feed.PublishAsync(envelope, cancellationToken);
    }
}

public class ListItemDeletedStreamHandler(ChangeFeed feed) : INotificationHandler<ListItemDeletedIntegrationEvent>
{
    public async Task Handle(ListItemDeletedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var envelope = new
            { type = nameof(ListItemDeletedIntegrationEvent), data = notification, occurredOn = DateTime.UtcNow };
        await feed.PublishAsync(envelope, cancellationToken);
    }
}

public class ListDeletedStreamHandler(ChangeFeed feed) : INotificationHandler<ListDeletedIntegrationEvent>
{
    public async Task Handle(ListDeletedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var envelope = new
            { type = nameof(ListDeletedIntegrationEvent), data = notification, occurredOn = DateTime.UtcNow };
        await feed.PublishAsync(envelope, cancellationToken);
    }
}

public class ListMigrationStartedStreamHandler(ChangeFeed feed)
    : INotificationHandler<ListMigrationStartedIntegrationEvent>
{
    public async Task Handle(ListMigrationStartedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var envelope = new
            { type = nameof(ListMigrationStartedIntegrationEvent), data = notification, occurredOn = DateTime.UtcNow };
        await feed.PublishAsync(envelope, cancellationToken);
    }
}

public class ListMigrationProgressStreamHandler(ChangeFeed feed)
    : INotificationHandler<ListMigrationProgressIntegrationEvent>
{
    public async Task Handle(ListMigrationProgressIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var envelope = new
            { type = nameof(ListMigrationProgressIntegrationEvent), data = notification, occurredOn = DateTime.UtcNow };
        await feed.PublishAsync(envelope, cancellationToken);
    }
}

public class ListMigrationCompletedStreamHandler(ChangeFeed feed)
    : INotificationHandler<ListMigrationCompletedIntegrationEvent>
{
    public async Task Handle(ListMigrationCompletedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var envelope = new
        {
            type = nameof(ListMigrationCompletedIntegrationEvent), data = notification, occurredOn = DateTime.UtcNow
        };
        await feed.PublishAsync(envelope, cancellationToken);
    }
}

public class ListMigrationFailedStreamHandler(ChangeFeed feed)
    : INotificationHandler<ListMigrationFailedIntegrationEvent>
{
    public async Task Handle(ListMigrationFailedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var envelope = new
            { type = nameof(ListMigrationFailedIntegrationEvent), data = notification, occurredOn = DateTime.UtcNow };
        await feed.PublishAsync(envelope, cancellationToken);
    }
}

public class ListUpdatedStreamHandler(ChangeFeed feed) : INotificationHandler<ListUpdatedIntegrationEvent>
{
    public async Task Handle(ListUpdatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var envelope = new
            { type = nameof(ListUpdatedIntegrationEvent), data = notification, occurredOn = DateTime.UtcNow };
        await feed.PublishAsync(envelope, cancellationToken);
    }
}
