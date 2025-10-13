using Lister.Core.Domain.IntegrationEvents;
using Lister.Lists.Domain.Events;
using MediatR;

namespace Lister.Lists.Application.EventHandlers;

public class PublishIntegrationEventHandler(
    ILogger<PublishIntegrationEventHandler> logger,
    IMediator mediator
) : INotificationHandler<ListCreatedEvent>,
    INotificationHandler<ListDeletedEvent>,
    INotificationHandler<ListItemCreatedEvent>,
    INotificationHandler<ListItemDeletedEvent>,
    INotificationHandler<ListItemUpdatedEvent>,
    INotificationHandler<ListUpdatedEvent>
{
    public async Task Handle(ListCreatedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("ListCreatedEvent: {notification}", new { notification.List.Id, notification.CreatedBy });

        if (notification.List.Id.HasValue)
        {
            var integrationEvent = new ListCreatedIntegrationEvent(
                notification.List.Id.Value,
                notification.List.Name,
                notification.CreatedBy
            );
            await mediator.Publish(integrationEvent, cancellationToken);
        }
    }

    public async Task Handle(ListDeletedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("ListDeletedEvent: {notification}", new { notification.List.Id, notification.DeletedBy });

        if (notification.List.Id.HasValue)
        {
            var integrationEvent = new ListDeletedIntegrationEvent(
                notification.List.Id.Value,
                notification.List.Name,
                notification.DeletedBy
            );
            await mediator.Publish(integrationEvent, cancellationToken);
        }
    }

    public async Task Handle(ListItemCreatedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("ListItemCreatedEvent: {notification}",
            new { notification.Item.Id, notification.CreatedBy });

        if (notification.Item.ListId.HasValue)
        {
            var integrationEvent = new ListItemCreatedIntegrationEvent(
                notification.Item.ListId.Value,
                notification.Item.Id,
                notification.CreatedBy
            );
            await mediator.Publish(integrationEvent, cancellationToken);
        }
    }

    public async Task Handle(ListItemDeletedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("ListItemDeletedEvent: {notification}",
            new { notification.Item.Id, notification.DeletedBy });

        if (notification.Item.ListId.HasValue)
        {
            var integrationEvent = new ListItemDeletedIntegrationEvent(
                notification.Item.ListId.Value,
                notification.Item.Id,
                notification.DeletedBy
            );
            await mediator.Publish(integrationEvent, cancellationToken);
        }
    }

    public async Task Handle(ListItemUpdatedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("ListItemUpdatedEvent: {notification}",
            new { notification.Item.Id, notification.UpdatedBy });

        if (notification.Item.ListId.HasValue)
        {
            var integrationEvent = new ListItemUpdatedIntegrationEvent(
                notification.Item.ListId.Value,
                notification.Item.Id,
                notification.UpdatedBy,
                notification.PreviousBag,
                notification.NewBag
            );
            await mediator.Publish(integrationEvent, cancellationToken);
        }
    }

    public async Task Handle(ListUpdatedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("ListUpdatedEvent: {notification}",
            new { notification.List.Id, notification.UpdatedBy });

        if (notification.List.Id.HasValue)
        {
            var integrationEvent = new ListUpdatedIntegrationEvent(
                notification.List.Id.Value,
                notification.List.Name,
                notification.UpdatedBy
            );
            await mediator.Publish(integrationEvent, cancellationToken);
        }
    }
}