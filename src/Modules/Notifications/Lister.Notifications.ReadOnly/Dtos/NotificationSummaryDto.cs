using System.Text.Json.Serialization;

namespace Lister.Notifications.ReadOnly.Dtos;

public record NotificationSummaryDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; init; }

    [JsonPropertyName("title")]
    public string Title { get; init; } = string.Empty;

    [JsonPropertyName("body")]
    public string Body { get; init; } = string.Empty;

    [JsonPropertyName("isRead")]
    public bool IsRead { get; init; }

    [JsonPropertyName("occurredOn")]
    public DateTime OccurredOn { get; init; }

    [JsonPropertyName("listId")]
    public Guid? ListId { get; init; }

    [JsonPropertyName("itemId")]
    public int? ItemId { get; init; }

    [JsonPropertyName("metadata")]
    public object? Metadata { get; init; }
}
