using System.Text.Json.Serialization;

namespace Lister.Notifications.ReadOnly.Dtos;

public record PendingNotificationDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; init; }

    [JsonPropertyName("userId")]
    public string UserId { get; init; } = string.Empty;

    [JsonPropertyName("listId")]
    public Guid ListId { get; init; }

    [JsonPropertyName("itemId")]
    public int? ItemId { get; init; }
}