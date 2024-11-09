using Lister.Lists.Domain.Entities;
using MediatR;

namespace Lister.Lists.Domain.Events;

public class ListItemCreatedEvent : INotification
{
    public ListItemCreatedEvent(Item item, string createdBy)
    {
        Item = item;
        CreatedBy = createdBy;
    }

    public Item Item { get; }

    public string CreatedBy { get; }
}