using System.Text.Json.Serialization;

namespace Lister.Notifications.Domain.Views;

public record NotificationListPage
{
    [JsonPropertyName("notifications")]
    public List<NotificationSummary> Notifications { get; init; } = [];

    [JsonPropertyName("totalCount")]
    public int TotalCount { get; init; }

    [JsonPropertyName("unreadCount")]
    public int UnreadCount { get; init; }

    [JsonPropertyName("hasMore")]
    public bool HasMore { get; init; }
}