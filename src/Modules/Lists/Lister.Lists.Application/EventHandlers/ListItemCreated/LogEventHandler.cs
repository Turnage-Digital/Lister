using Lister.Lists.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Lister.Lists.Application.EventHandlers.ListItemCreated;

public class LogEventHandler(ILogger<LogEventHandler> logger) : INotificationHandler<ListItemCreatedEvent>
{
    public Task Handle(ListItemCreatedEvent notification, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("ListItemCreatedEvent: {notification}", notification);
        return Task.CompletedTask;
    }
}