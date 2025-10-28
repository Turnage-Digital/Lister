using Lister.Lists.Domain.Entities;
using MediatR;

namespace Lister.Lists.Domain.Events;

public class ListItemUpdatedEvent : INotification
{
    public ListItemUpdatedEvent(IWritableItem item, string updatedBy, object? previousBag, object newBag)
    {
        Item = item;
        UpdatedBy = updatedBy;
        PreviousBag = previousBag;
        NewBag = newBag;
    }

    public IWritableItem Item { get; }

    public string UpdatedBy { get; }

    public object? PreviousBag { get; }

    public object NewBag { get; }
}