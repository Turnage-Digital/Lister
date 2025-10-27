using System.Text.Json.Serialization;
using Lister.Notifications.Domain.Enums;

namespace Lister.Notifications.ReadOnly.Dtos;

public record NotificationChannelDto
{
    [JsonPropertyName("type")]
    public ChannelType Type { get; init; }

    [JsonPropertyName("address")]
    public string? Address { get; init; }

    [JsonPropertyName("settings")]
    public Dictionary<string, string> Settings { get; init; } = new();
}
