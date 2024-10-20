using Lister.Lists.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Lister.Lists.Application.Events.Handlers;

public class ListItemsAddedEventHandler(ILogger<ListItemsAddedEventHandler> logger)
    : INotificationHandler<ListItemsAddedEvent>
{
    public Task Handle(ListItemsAddedEvent notification, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Added list items: {items}", new { notification.Ids, notification.AddedBy });
        return Task.CompletedTask;
    }
}