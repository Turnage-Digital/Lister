using Lister.Lists.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Lister.Lists.Application.EventHandlers.ListItemDeleted;

public class LogEventHandler(ILogger<LogEventHandler> logger) : INotificationHandler<ListItemDeletedEvent>
{
    public Task Handle(ListItemDeletedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("ListItemDeletedEvent: {notification}",
            new { notification.Item.Id, notification.DeletedBy });
        return Task.CompletedTask;
    }
}