namespace Lister.Core.Domain.IntegrationEvents;

public class ListMigrationFailedIntegrationEvent : IIntegrationEvent
{
    public ListMigrationFailedIntegrationEvent(Guid listId, Guid jobId, Guid correlationId, string message)
    {
        EventId = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
        EventType = nameof(ListMigrationFailedIntegrationEvent);
        ListId = listId;
        JobId = jobId;
        CorrelationId = correlationId;
        Message = message;
    }

    public Guid ListId { get; }
    public Guid JobId { get; }
    public Guid CorrelationId { get; }
    public string Message { get; }
    public Guid EventId { get; }
    public DateTime OccurredOn { get; }
    public string EventType { get; }
}