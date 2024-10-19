using MediatR;

namespace Lister.Domain.Events.List;

public class ListItemAddedEvent(int id, string addedBy) : INotification
{
    public int Id { get; } = id;

    public string AddedBy { get; } = addedBy;
}