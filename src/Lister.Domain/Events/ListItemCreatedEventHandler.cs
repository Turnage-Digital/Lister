using MediatR;
using Serilog;

namespace Lister.Domain.Events;

public class ListItemCreatedEventHandler : INotificationHandler<ListItemCreatedEvent>
{
    public Task Handle(ListItemCreatedEvent notification, CancellationToken cancellationToken = default)
    {
        Log.Information("ListItemCreatedEvent: {Id}", notification.Id);
        return Task.CompletedTask;
    }
}