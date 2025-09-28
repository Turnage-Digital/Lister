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

public class ListUpdatedStreamHandler(ChangeFeed feed) : INotificationHandler<ListUpdatedIntegrationEvent>
{
    public async Task Handle(ListUpdatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var envelope = new
            { type = nameof(ListUpdatedIntegrationEvent), data = notification, occurredOn = DateTime.UtcNow };
        await feed.PublishAsync(envelope, cancellationToken);
    }
}