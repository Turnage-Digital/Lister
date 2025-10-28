namespace Lister.Core.Domain.IntegrationEvents;

public class ListDeletedIntegrationEvent : IIntegrationEvent
{
    public ListDeletedIntegrationEvent(
        Guid listId,
        string listName,
        string deletedBy
    )
    {
        EventId = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
        EventType = nameof(ListDeletedIntegrationEvent);
        ListId = listId;
        ListName = listName;
        DeletedBy = deletedBy;
    }

    public Guid ListId { get; }
    public string ListName { get; }
    public string DeletedBy { get; }

    public Guid EventId { get; }
    public DateTime OccurredOn { get; }
    public string EventType { get; }
}