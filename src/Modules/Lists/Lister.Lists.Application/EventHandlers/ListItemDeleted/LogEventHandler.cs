using Lister.Core.Domain.IntegrationEvents;
using Lister.Lists.Domain.Events;
using MediatR;

namespace Lister.Lists.Application.EventHandlers.ListItemDeleted;

public class LogEventHandler(
    ILogger<LogEventHandler> logger,
    IMediator mediator
) : INotificationHandler<ListItemDeletedEvent>
{
    public async Task Handle(ListItemDeletedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("ListItemDeletedEvent: {notification}",
            new { notification.Item.Id, notification.DeletedBy });

        // Publish integration event for other modules
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
}