using Lister.Lists.Domain.Entities;
using MediatR;

namespace Lister.Lists.Domain.Events;

public class ListItemDeletedEvent : INotification
{
    public ListItemDeletedEvent(IWritableItem item, string deletedBy)
    {
        Item = item;
        DeletedBy = deletedBy;
    }

    public IWritableItem Item { get; }

    public string DeletedBy { get; }
}