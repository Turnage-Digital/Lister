using MediatR;
using Serilog;

namespace Lister.Domain.Events;

public class ListItemsAddedEventHandler : INotificationHandler<ListItemsAddedEvent>
{
    public Task Handle(ListItemsAddedEvent notification, CancellationToken cancellationToken = default)
    {
        Log.Information("Added list items: {items}", new { notification.Ids, notification.AddedBy });
        return Task.CompletedTask;
    }
}