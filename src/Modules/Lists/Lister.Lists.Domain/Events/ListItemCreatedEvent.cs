using Lister.Lists.Domain.Entities;
using MediatR;

namespace Lister.Lists.Domain.Events;

public class ListItemCreatedEvent : INotification
{
    public ListItemCreatedEvent(IItem item, string createdBy)
    {
        Item = item;
        CreatedBy = createdBy;
    }

    public IItem Item { get; }

    public string CreatedBy { get; }
}