using System.Text.Json.Serialization;

namespace Lister.Notifications.Domain.ValueObjects;

public record NotificationContent
{
    [JsonPropertyName("subject")]
    public string Subject { get; init; } = string.Empty;

    [JsonPropertyName("body")]
    public string Body { get; init; } = string.Empty;

    [JsonPropertyName("data")]
    public Dictionary<string, object> Data { get; init; } = new();

    [JsonPropertyName("listName")]
    public string? ListName { get; init; }

    [JsonPropertyName("itemIdentifier")]
    public string? ItemIdentifier { get; init; }

    [JsonPropertyName("oldValue")]
    public string? OldValue { get; init; }

    [JsonPropertyName("newValue")]
    public string? NewValue { get; init; }

    [JsonPropertyName("triggeringUser")]
    public string? TriggeringUser { get; init; }

    [JsonPropertyName("occurredOn")]
    public DateTime OccurredOn { get; init; }
}