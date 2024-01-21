using MediatR;

namespace Lister.Domain.Events;

public class ListCreatedEvent : INotification
{
    public ListCreatedEvent(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; }
}