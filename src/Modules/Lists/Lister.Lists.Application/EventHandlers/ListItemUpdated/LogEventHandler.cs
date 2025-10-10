using Lister.Core.Domain.IntegrationEvents;
using Lister.Lists.Domain.Events;
using MediatR;

namespace Lister.Lists.Application.EventHandlers.ListItemUpdated;

public class LogEventHandler(
    ILogger<LogEventHandler> logger,
    IMediator mediator
) : INotificationHandler<ListItemUpdatedEvent>
{
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
}
