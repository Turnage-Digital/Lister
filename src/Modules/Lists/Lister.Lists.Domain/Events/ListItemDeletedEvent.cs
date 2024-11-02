using MediatR;

namespace Lister.Lists.Domain.Events;

public class ListItemDeletedEvent(int id, string deletedBy) : INotification
{
    public int Id { get; } = id;

    public string DeletedBy { get; } = deletedBy;
}