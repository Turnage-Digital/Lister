namespace Lister.Core.Domain.IntegrationEvents;

public class ListMigrationCompletedIntegrationEvent : IIntegrationEvent
{
    public ListMigrationCompletedIntegrationEvent(
        Guid listId,
        Guid jobId,
        Guid correlationId,
        string completedBy,
        int itemsProcessed
    )
    {
        EventId = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
        EventType = nameof(ListMigrationCompletedIntegrationEvent);
        ListId = listId;
        JobId = jobId;
        CorrelationId = correlationId;
        CompletedBy = completedBy;
        ItemsProcessed = itemsProcessed;
    }

    public Guid ListId { get; }
    public Guid JobId { get; }
    public Guid CorrelationId { get; }
    public string CompletedBy { get; }
    public int ItemsProcessed { get; }
    public Guid EventId { get; }
    public DateTime OccurredOn { get; }
    public string EventType { get; }
}