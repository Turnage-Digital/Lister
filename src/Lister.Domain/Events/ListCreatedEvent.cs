using MediatR;

namespace Lister.Domain.Events;

public class ListCreatedEvent : INotification
{
    public Guid Id { get; set; }
}