using MediatR;

namespace Lister.Lists.Domain.Events;

public class ListCreatedEvent : INotification
{
    public ListCreatedEvent(IList list, string createdBy)
    {
        List = list;
        CreatedBy = createdBy;
    }

    public IList List { get; }

    public string CreatedBy { get; }
}