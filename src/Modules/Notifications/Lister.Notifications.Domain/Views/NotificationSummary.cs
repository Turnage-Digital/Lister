using System.Text.Json.Serialization;

namespace Lister.Notifications.Domain.Views;

public record NotificationSummary
{
    [JsonPropertyName("id")]
    public Guid Id { get; init; }

    [JsonPropertyName("notificationRuleId")]
    public Guid? NotificationRuleId { get; init; }

    [JsonPropertyName("userId")]
    public string UserId { get; init; } = string.Empty;

    [JsonPropertyName("listId")]
    public Guid ListId { get; init; }

    [JsonPropertyName("itemId")]
    public int? ItemId { get; init; }

    [JsonPropertyName("title")]
    public string Title { get; init; } = string.Empty;

    [JsonPropertyName("body")]
    public string Body { get; init; } = string.Empty;

    [JsonPropertyName("isRead")]
    public bool IsRead { get; init; }

    [JsonPropertyName("metadata")]
    public object? Metadata { get; init; }
}