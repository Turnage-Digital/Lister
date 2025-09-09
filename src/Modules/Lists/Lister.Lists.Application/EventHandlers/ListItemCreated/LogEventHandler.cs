using Lister.Core.Domain.IntegrationEvents;
using Lister.Lists.Domain.Events;
using MediatR;

namespace Lister.Lists.Application.EventHandlers.ListItemCreated;

public class LogEventHandler(
    ILogger<LogEventHandler> logger,
    IMediator mediator
) : INotificationHandler<ListItemCreatedEvent>
{
    public async Task Handle(ListItemCreatedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("ListItemCreatedEvent: {notification}",
            new { notification.Item.Id, notification.CreatedBy });

        // Publish integration event for other modules
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
}