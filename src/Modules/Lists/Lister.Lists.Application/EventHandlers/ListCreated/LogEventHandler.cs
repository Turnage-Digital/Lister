using Lister.Lists.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Lister.Lists.Application.EventHandlers.ListCreated;

public class LogEventHandler(ILogger<LogEventHandler> logger) : INotificationHandler<ListCreatedEvent>
{
    public Task Handle(ListCreatedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("ListCreatedEvent: {notification}", new { notification.List.Id, notification.CreatedBy });
        return Task.CompletedTask;
    }
}