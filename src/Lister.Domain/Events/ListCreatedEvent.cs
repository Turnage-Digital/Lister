using MediatR;

namespace Lister.Domain.Events.List;

public class ListCreatedEvent(Guid id, string createdBy) : INotification
{
    public Guid Id { get; } = id;

    public string CreatedBy { get; } = createdBy;
}