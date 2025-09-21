using MediatR;

namespace Lister.Lists.Domain.Events;

public class ListItemDeletedEvent : INotification
{
    public ListItemDeletedEvent(IItem item, string deletedBy)
    {
        Item = item;
        DeletedBy = deletedBy;
    }

    public IItem Item { get; }

    public string DeletedBy { get; }
}