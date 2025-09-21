using Lister.Core.Domain.IntegrationEvents;
using Lister.Lists.Domain.Events;
using MediatR;

namespace Lister.Lists.Application.EventHandlers.ListCreated;

public class LogEventHandler(
    ILogger<LogEventHandler> logger,
    IMediator mediator
) : INotificationHandler<ListCreatedEvent>
{
    public async Task Handle(ListCreatedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("ListCreatedEvent: {notification}", new { notification.List.Id, notification.CreatedBy });

        // Publish integration event for other modules
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
}