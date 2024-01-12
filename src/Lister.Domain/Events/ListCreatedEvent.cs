using MediatR;

namespace Lister.Domain.Events;

public class ListCreatedEvent : INotification
{
    public ListCreatedEvent(string id)
    {
        Id = id;
    }

    public string Id { get; }
}