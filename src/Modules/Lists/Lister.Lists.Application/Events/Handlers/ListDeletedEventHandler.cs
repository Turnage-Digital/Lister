using Lister.Lists.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Lister.Lists.Application.Events.Handlers;

public class ListDeletedEventHandler(ILogger<ListDeletedEventHandler> logger) : INotificationHandler<ListDeletedEvent>
{
    public Task Handle(ListDeletedEvent notification, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Deleted list: {list}", new { notification.Id, notification.DeletedBy });
        return Task.CompletedTask;
    }
}