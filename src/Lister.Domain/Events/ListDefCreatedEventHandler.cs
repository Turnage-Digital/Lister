using MediatR;
using Serilog;

namespace Lister.Domain.Events;

public class ListDefCreatedEventHandler : INotificationHandler<ListDefCreatedEvent>
{
    public Task Handle(ListDefCreatedEvent notification, CancellationToken cancellationToken = default)
    {
        Log.Information("ListDefCreatedEvent: {Id}", notification.Id);
        return Task.CompletedTask;
    }
}