namespace Lister.Core.Domain.IntegrationEvents;

public class ListItemCreatedIntegrationEvent : IIntegrationEvent
{
    public ListItemCreatedIntegrationEvent(
        Guid listId,
        int? itemId,
        string createdBy
    )
    {
        EventId = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
        EventType = nameof(ListItemCreatedIntegrationEvent);
        ListId = listId;
        ItemId = itemId;
        CreatedBy = createdBy;
    }

    public Guid ListId { get; }
    public int? ItemId { get; }
    public string CreatedBy { get; }

    public Guid EventId { get; }
    public DateTime OccurredOn { get; }
    public string EventType { get; }
}
