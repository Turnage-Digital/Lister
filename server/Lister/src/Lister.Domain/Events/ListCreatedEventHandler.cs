using MediatR;
using Serilog;

namespace Lister.Domain.Events;

public class ListCreatedEventHandler : INotificationHandler<ListCreatedEvent>
{
    public Task Handle(ListCreatedEvent notification, CancellationToken cancellationToken = default)
    {
        Log.Information("ListCreatedEvent: {Id}", notification.Id);
        return Task.CompletedTask;
    }
}