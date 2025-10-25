namespace Lister.Core.Domain.IntegrationEvents;

public class ListMigrationProgressIntegrationEvent : IIntegrationEvent
{
    public ListMigrationProgressIntegrationEvent(
        Guid listId,
        Guid jobId,
        Guid correlationId,
        string message,
        int percent
    )
    {
        EventId = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
        EventType = nameof(ListMigrationProgressIntegrationEvent);
        ListId = listId;
        JobId = jobId;
        CorrelationId = correlationId;
        Message = message;
        Percent = percent;
    }

    public Guid ListId { get; }
    public Guid JobId { get; }
    public Guid CorrelationId { get; }
    public string Message { get; }
    public int Percent { get; }
    public Guid EventId { get; }
    public DateTime OccurredOn { get; }
    public string EventType { get; }
}