using MediatR;

namespace Lister.Domain.Events;

public class ListItemCreatedEvent : INotification
{
    public ListItemCreatedEvent(string id)
    {
        Id = id;
    }

    public string Id { get; }
}