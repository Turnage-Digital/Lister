using MediatR;
using Microsoft.Extensions.Logging;

namespace Lister.Domain.Events;

public class ListItemAddedEventHandler(ILogger<ListItemAddedEventHandler> logger)
    : INotificationHandler<ListItemAddedEvent>
{
    public Task Handle(ListItemAddedEvent notification, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Added list item: {item}", new { notification.Id, notification.AddedBy });
        return Task.CompletedTask;
    }
}