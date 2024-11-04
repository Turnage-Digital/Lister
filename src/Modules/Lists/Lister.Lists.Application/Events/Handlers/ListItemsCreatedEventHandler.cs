using Lister.Lists.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Lister.Lists.Application.Events.Handlers;

public class ListItemsCreatedEventHandler(ILogger<ListItemsCreatedEventHandler> logger)
    : INotificationHandler<ListItemsCreatedEvent>
{
    public Task Handle(ListItemsCreatedEvent notification, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Added list items: {items}", new { notification.Ids, notification.AddedBy });
        return Task.CompletedTask;
    }
}