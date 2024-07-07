using MediatR;
using Serilog;

namespace Lister.Domain.Events;

public class ListItemAddedEventHandler : INotificationHandler<ListItemAddedEvent>
{
    public Task Handle(ListItemAddedEvent notification, CancellationToken cancellationToken = default)
    {
        Log.Information("Added list item: {item}", new { notification.Id, notification.AddedBy });
        return Task.CompletedTask;
    }
}