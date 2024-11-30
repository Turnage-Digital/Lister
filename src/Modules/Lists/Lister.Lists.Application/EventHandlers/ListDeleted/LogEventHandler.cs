using Lister.Lists.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Lister.Lists.Application.EventHandlers.ListDeleted;

public class LogEventHandler(ILogger<LogEventHandler> logger) : INotificationHandler<ListDeletedEvent>
{
    public Task Handle(ListDeletedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("ListDeletedEvent: {notification}", new { notification.List.Id, notification.DeletedBy });
        return Task.CompletedTask;
    }
}