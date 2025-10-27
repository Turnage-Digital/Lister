using System.Text.Json.Serialization;

namespace Lister.Notifications.ReadOnly.Dtos;

public record NotificationRuleDto
{
    [JsonPropertyName("trigger")]
    public NotificationTriggerDto Trigger { get; init; } = null!;

    [JsonPropertyName("channels")]
    public NotificationChannelDto[] Channels { get; init; } = [];

    [JsonPropertyName("schedule")]
    public NotificationScheduleDto Schedule { get; init; } = null!;

    [JsonPropertyName("isActive")]
    public bool IsActive { get; init; }

    [JsonPropertyName("templateId")]
    public string? TemplateId { get; init; }

    [JsonPropertyName("id")]
    public Guid? Id { get; init; }

    [JsonPropertyName("userId")]
    public string UserId { get; init; } = string.Empty;

    [JsonPropertyName("listId")]
    public Guid ListId { get; init; }
}
