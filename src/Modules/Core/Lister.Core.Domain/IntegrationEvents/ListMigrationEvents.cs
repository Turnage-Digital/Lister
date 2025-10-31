namespace Lister.Core.Domain.IntegrationEvents;

public class ListMigrationRequestedIntegrationEvent : IIntegrationEvent
{
    public ListMigrationRequestedIntegrationEvent(
        Guid listId,
        Guid correlationId,
        string requestedBy,
        string planJson
    )
    {
        EventId = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
        EventType = nameof(ListMigrationRequestedIntegrationEvent);
        ListId = listId;
        CorrelationId = correlationId;
        RequestedBy = requestedBy;
        PlanJson = planJson;
    }

    public Guid ListId { get; }
    public Guid CorrelationId { get; }
    public string RequestedBy { get; }
    public string PlanJson { get; }
    public Guid EventId { get; }
    public DateTime OccurredOn { get; }
    public string EventType { get; }
}

public class ListMigrationStartedIntegrationEvent : IIntegrationEvent
{
    public ListMigrationStartedIntegrationEvent(Guid listId, Guid correlationId, string startedBy)
    {
        EventId = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
        EventType = nameof(ListMigrationStartedIntegrationEvent);
        ListId = listId;
        CorrelationId = correlationId;
        StartedBy = startedBy;
    }

    public Guid ListId { get; }
    public Guid CorrelationId { get; }
    public string StartedBy { get; }
    public Guid EventId { get; }
    public DateTime OccurredOn { get; }
    public string EventType { get; }
}

public class ListMigrationProgressIntegrationEvent : IIntegrationEvent
{
    public ListMigrationProgressIntegrationEvent(Guid listId, Guid correlationId, string message, int percent)
    {
        EventId = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
        EventType = nameof(ListMigrationProgressIntegrationEvent);
        ListId = listId;
        CorrelationId = correlationId;
        Message = message;
        Percent = percent;
    }

    public Guid ListId { get; }
    public Guid CorrelationId { get; }
    public string Message { get; }
    public int Percent { get; }
    public Guid EventId { get; }
    public DateTime OccurredOn { get; }
    public string EventType { get; }
}

public class ListMigrationCompletedIntegrationEvent : IIntegrationEvent
{
    public ListMigrationCompletedIntegrationEvent(
        Guid listId,
        Guid correlationId,
        string completedBy,
        int itemsProcessed
    )
    {
        EventId = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
        EventType = nameof(ListMigrationCompletedIntegrationEvent);
        ListId = listId;
        CorrelationId = correlationId;
        CompletedBy = completedBy;
        ItemsProcessed = itemsProcessed;
    }

    public Guid ListId { get; }
    public Guid CorrelationId { get; }
    public string CompletedBy { get; }
    public int ItemsProcessed { get; }
    public Guid EventId { get; }
    public DateTime OccurredOn { get; }
    public string EventType { get; }
}

public class ListMigrationFailedIntegrationEvent : IIntegrationEvent
{
    public ListMigrationFailedIntegrationEvent(Guid listId, Guid correlationId, string message)
    {
        EventId = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
        EventType = nameof(ListMigrationFailedIntegrationEvent);
        ListId = listId;
        CorrelationId = correlationId;
        Message = message;
    }

    public Guid ListId { get; }
    public Guid CorrelationId { get; }
    public string Message { get; }
    public Guid EventId { get; }
    public DateTime OccurredOn { get; }
    public string EventType { get; }
}