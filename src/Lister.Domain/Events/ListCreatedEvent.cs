using MediatR;

namespace Lister.Domain.Events;

public class ListCreatedEvent(Guid id) : INotification
{
    public Guid Id { get; } = id;
}