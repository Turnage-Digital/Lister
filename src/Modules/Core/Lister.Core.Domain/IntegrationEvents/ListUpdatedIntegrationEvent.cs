namespace Lister.Core.Domain.IntegrationEvents;

public class ListUpdatedIntegrationEvent : IIntegrationEvent
{
    public ListUpdatedIntegrationEvent(
        Guid listId,
        string listName,
        string updatedBy
    )
    {
        EventId = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
        EventType = nameof(ListUpdatedIntegrationEvent);
        ListId = listId;
        ListName = listName;
        UpdatedBy = updatedBy;
    }

    public Guid ListId { get; }
    public string ListName { get; }
    public string UpdatedBy { get; }

    public Guid EventId { get; }
    public DateTime OccurredOn { get; }
    public string EventType { get; }
}