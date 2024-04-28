using MediatR;

namespace Lister.Domain.Events;

public class ListItemCreatedEvent(int id) : INotification
{
    public int Id { get; } = id;
}