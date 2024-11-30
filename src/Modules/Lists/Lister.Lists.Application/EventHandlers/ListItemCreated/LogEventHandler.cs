using Lister.Lists.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Lister.Lists.Application.EventHandlers.ListItemCreated;

public class LogEventHandler(ILogger<LogEventHandler> logger) : INotificationHandler<ListItemCreatedEvent>
{
    public Task Handle(ListItemCreatedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("ListItemCreatedEvent: {notification}", new { notification.Item.Id, notification.CreatedBy });
        return Task.CompletedTask;
    }
}