using Lister.Lists.Domain.Entities;
using MediatR;

namespace Lister.Lists.Domain.Events;

public class ListDeletedEvent : INotification
{
    public ListDeletedEvent(IWritableList list, string deletedBy)
    {
        List = list;
        DeletedBy = deletedBy;
    }

    public IWritableList List { get; }

    public string DeletedBy { get; }
}