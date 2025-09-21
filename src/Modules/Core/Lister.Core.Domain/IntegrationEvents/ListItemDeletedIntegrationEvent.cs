namespace Lister.Core.Domain.IntegrationEvents;

public class ListItemDeletedIntegrationEvent : IIntegrationEvent
{
    public ListItemDeletedIntegrationEvent(
        Guid listId,
        int? itemId,
        string deletedBy
    )
    {
        EventId = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
        EventType = nameof(ListItemDeletedIntegrationEvent);
        ListId = listId;
        ItemId = itemId;
        DeletedBy = deletedBy;
    }

    public Guid ListId { get; }
    public int? ItemId { get; }
    public string DeletedBy { get; }

    public Guid EventId { get; }
    public DateTime OccurredOn { get; }
    public string EventType { get; }
}