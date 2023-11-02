using MediatR;

namespace Lister.Domain.Events;

public class ListDefCreatedEvent : INotification
{
    public Guid Id { get; set; }
}