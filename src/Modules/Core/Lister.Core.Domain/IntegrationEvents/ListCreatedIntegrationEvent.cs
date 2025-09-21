namespace Lister.Core.Domain.IntegrationEvents;

public class ListCreatedIntegrationEvent : IIntegrationEvent
{
    public ListCreatedIntegrationEvent(
        Guid listId,
        string listName,
        string createdBy
    )
    {
        EventId = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
        EventType = nameof(ListCreatedIntegrationEvent);
        ListId = listId;
        ListName = listName;
        CreatedBy = createdBy;
    }

    public Guid ListId { get; }
    public string ListName { get; }
    public string CreatedBy { get; }

    public Guid EventId { get; }
    public DateTime OccurredOn { get; }
    public string EventType { get; }
}