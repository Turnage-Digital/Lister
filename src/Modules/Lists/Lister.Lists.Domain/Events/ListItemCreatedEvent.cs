using MediatR;

namespace Lister.Lists.Domain.Events;

public class ListItemCreatedEvent(int id, string addedBy) : INotification
{
    public int Id { get; } = id;

    public string AddedBy { get; } = addedBy;
}