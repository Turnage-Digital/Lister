using Lister.Core.Domain.IntegrationEvents;
using Lister.Lists.Domain.Events;
using MediatR;

namespace Lister.Lists.Application.EventHandlers.ListUpdated;

public class LogEventHandler(
    ILogger<LogEventHandler> logger,
    IMediator mediator
) : INotificationHandler<ListUpdatedEvent>
{
    public async Task Handle(ListUpdatedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("ListUpdatedEvent: {notification}", new { notification.List.Id, notification.UpdatedBy });

        // Publish integration event for other modules
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