using Lister.Lists.Domain.Entities;
using MediatR;

namespace Lister.Lists.Domain.Events;

public class ListUpdatedEvent : INotification
{
    public ListUpdatedEvent(IWritableList list, string updatedBy)
    {
        List = list;
        UpdatedBy = updatedBy;
    }

    public IWritableList List { get; }

    public string UpdatedBy { get; }
}