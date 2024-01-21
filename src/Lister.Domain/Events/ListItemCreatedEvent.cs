using MediatR;

namespace Lister.Domain.Events;

public class ListItemCreatedEvent : INotification
{
    public ListItemCreatedEvent(int id)
    {
        Id = id;
    }

    public int Id { get; }
}