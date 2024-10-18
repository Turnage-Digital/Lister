using MediatR;
using Microsoft.Extensions.Logging;

namespace Lister.Domain.Events.Handlers;

public class ListCreatedEventHandler(ILogger<ListCreatedEventHandler> logger)
    : INotificationHandler<ListCreatedEvent>
{
    public Task Handle(ListCreatedEvent notification, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Created list: {list}", new { notification.Id, notification.CreatedBy });
        return Task.CompletedTask;
    }
}