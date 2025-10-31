using System.Text.Json.Serialization;

namespace Lister.Notifications.ReadOnly.Dtos;

public record NotificationListPageDto
{
    [JsonPropertyName("notifications")]
    public List<NotificationSummaryDto> Notifications { get; init; } = [];

    [JsonPropertyName("totalCount")]
    public int TotalCount { get; init; }

    [JsonPropertyName("unreadCount")]
    public int UnreadCount { get; init; }

    [JsonPropertyName("hasMore")]
    public bool HasMore { get; init; }
}