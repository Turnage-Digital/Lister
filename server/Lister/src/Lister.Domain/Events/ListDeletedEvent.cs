using MediatR;

namespace Lister.Domain.Events;

public class ListDeletedEvent(Guid id, string deletedBy) : INotification
{
    public Guid Id { get; } = id;
    
    public string DeletedBy { get; } = deletedBy;
}