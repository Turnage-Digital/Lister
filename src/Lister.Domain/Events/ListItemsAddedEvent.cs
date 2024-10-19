using MediatR;

namespace Lister.Domain.Events.List;

public class ListItemsAddedEvent(IEnumerable<int> ids, string addedBy) : INotification
{
    public IEnumerable<int> Ids { get; } = ids;

    public string AddedBy { get; } = addedBy;
}