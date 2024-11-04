using Lister.Lists.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Lister.Lists.Application.Events.Handlers;

public class ListItemCreatedEventHandler(ILogger<ListItemCreatedEventHandler> logger)
    : INotificationHandler<ListItemCreatedEvent>
{
    public Task Handle(ListItemCreatedEvent notification, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Added list item: {item}", new { notification.Id, notification.AddedBy });
        return Task.CompletedTask;
    }
}