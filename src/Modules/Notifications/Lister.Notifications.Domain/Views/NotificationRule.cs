using System.Text.Json.Serialization;
using Lister.Notifications.Domain.ValueObjects;

namespace Lister.Notifications.Domain.Views;

public class NotificationRule : IReadOnlyNotificationRule
{
    [JsonPropertyName("trigger")]
    public NotificationTrigger Trigger { get; set; } = null!;

    [JsonPropertyName("channels")]
    public NotificationChannel[] Channels { get; set; } = null!;

    [JsonPropertyName("schedule")]
    public NotificationSchedule Schedule { get; set; } = null!;

    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; }

    [JsonPropertyName("templateId")]
    public string? TemplateId { get; set; }

    [JsonPropertyName("id")]
    public Guid? Id { get; set; }

    [JsonPropertyName("userId")]
    public string UserId { get; set; } = null!;

    [JsonPropertyName("listId")]
    public Guid ListId { get; set; }
}