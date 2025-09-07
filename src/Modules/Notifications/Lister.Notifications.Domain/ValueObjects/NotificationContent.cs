namespace Lister.Notifications.Domain.ValueObjects;

public record NotificationContent
{
    public string Subject { get; init; } = string.Empty;
    public string Body { get; init; } = string.Empty;
    public Dictionary<string, object> Data { get; init; } = new();
    public string? ListName { get; init; }
    public string? ItemIdentifier { get; init; }
    public string? OldValue { get; init; }
    public string? NewValue { get; init; }
    public string? TriggeringUser { get; init; }
    public DateTime OccurredOn { get; init; }
}