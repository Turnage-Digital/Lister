using Lister.Lists.Domain.Entities;
using MediatR;

namespace Lister.Lists.Domain.Events;

public class ListItemDeletedEvent : INotification
{
    public ListItemDeletedEvent(Item item, string deletedBy)
    {
        Item = item;
        DeletedBy = deletedBy;
    }
    
    public Item Item { get; }

    public string DeletedBy { get; }
}