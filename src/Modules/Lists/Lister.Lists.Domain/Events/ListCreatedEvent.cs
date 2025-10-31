using Lister.Lists.Domain.Entities;
using MediatR;

namespace Lister.Lists.Domain.Events;

public class ListCreatedEvent : INotification
{
    public ListCreatedEvent(IWritableList list, string createdBy)
    {
        List = list;
        CreatedBy = createdBy;
    }

    public IWritableList List { get; }

    public string CreatedBy { get; }
}