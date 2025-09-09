using Lister.Core.Domain.IntegrationEvents;
using Lister.Lists.Domain.Events;
using MediatR;

namespace Lister.Lists.Application.EventHandlers.ListDeleted;

public class LogEventHandler(
    ILogger<LogEventHandler> logger,
    IMediator mediator
) : INotificationHandler<ListDeletedEvent>
{
    public async Task Handle(ListDeletedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("ListDeletedEvent: {notification}", new { notification.List.Id, notification.DeletedBy });

        // Publish integration event for other modules
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
}