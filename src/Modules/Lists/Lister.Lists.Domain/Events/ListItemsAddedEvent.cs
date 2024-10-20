using MediatR;

namespace Lister.Lists.Domain.Events;

public class ListItemsAddedEvent(IEnumerable<int> ids, string addedBy) : INotification
{
    public IEnumerable<int> Ids { get; } = ids;

    public string AddedBy { get; } = addedBy;
}