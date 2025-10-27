namespace Lister.Core.Domain.IntegrationEvents;

public class ListItemUpdatedIntegrationEvent : IIntegrationEvent
{
    public ListItemUpdatedIntegrationEvent(
        Guid listId,
        int? itemId,
        string updatedBy,
        object? previousBag,
        object newBag
    )
    {
        EventId = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
        EventType = nameof(ListItemUpdatedIntegrationEvent);
        ListId = listId;
        ItemId = itemId;
        UpdatedBy = updatedBy;
        PreviousBag = previousBag;
        NewBag = newBag;
    }

    public Guid ListId { get; }
    public int? ItemId { get; }
    public string UpdatedBy { get; }
    public object? PreviousBag { get; }
    public object NewBag { get; }

    public Guid EventId { get; }
    public DateTime OccurredOn { get; }
    public string EventType { get; }
}
