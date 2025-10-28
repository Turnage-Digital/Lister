using Lister.Lists.Domain.Entities;
using MediatR;

namespace Lister.Lists.Domain.Events;

public class ListItemCreatedEvent : INotification
{
    public ListItemCreatedEvent(IWritableItem item, string createdBy)
    {
        Item = item;
        CreatedBy = createdBy;
    }

    public IWritableItem Item { get; }

    public string CreatedBy { get; }
}