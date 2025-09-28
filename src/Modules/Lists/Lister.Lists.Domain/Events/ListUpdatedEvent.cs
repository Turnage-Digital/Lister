using MediatR;

namespace Lister.Lists.Domain.Events;

public class ListUpdatedEvent : INotification
{
    public ListUpdatedEvent(IList list, string updatedBy)
    {
        List = list;
        UpdatedBy = updatedBy;
    }

    public IList List { get; }

    public string UpdatedBy { get; }
}