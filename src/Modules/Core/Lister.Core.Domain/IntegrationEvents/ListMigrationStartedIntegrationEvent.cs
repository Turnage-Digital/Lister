namespace Lister.Core.Domain.IntegrationEvents;

public class ListMigrationStartedIntegrationEvent : IIntegrationEvent
{
    public ListMigrationStartedIntegrationEvent(Guid listId, Guid jobId, Guid correlationId, string startedBy)
    {
        EventId = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
        EventType = nameof(ListMigrationStartedIntegrationEvent);
        ListId = listId;
        JobId = jobId;
        CorrelationId = correlationId;
        StartedBy = startedBy;
    }

    public Guid ListId { get; }
    public Guid JobId { get; }
    public Guid CorrelationId { get; }
    public string StartedBy { get; }
    public Guid EventId { get; }
    public DateTime OccurredOn { get; }
    public string EventType { get; }
}