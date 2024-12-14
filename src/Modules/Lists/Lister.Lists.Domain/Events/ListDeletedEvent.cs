using Lister.Lists.Domain.Entities;
using MediatR;

namespace Lister.Lists.Domain.Events;

public class ListDeletedEvent : INotification
{
    public ListDeletedEvent(IList list, string deletedBy)
    {
        List = list;
        DeletedBy = deletedBy;
    }

    public IList List { get; }

    public string DeletedBy { get; }
}