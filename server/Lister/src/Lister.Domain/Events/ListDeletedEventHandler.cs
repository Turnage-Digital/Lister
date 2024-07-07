using MediatR;
using Serilog;

namespace Lister.Domain.Events;

public class ListDeletedEventHandler : INotificationHandler<ListDeletedEvent>
{
    public Task Handle(ListDeletedEvent notification, CancellationToken cancellationToken = default)
    {
        Log.Information("Deleted list: {list}", new { notification.Id, notification.DeletedBy });
        return Task.CompletedTask;
    }
}