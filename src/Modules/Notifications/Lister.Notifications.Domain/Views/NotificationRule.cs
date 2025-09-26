using System.Text.Json.Serialization;

namespace Lister.Notifications.Domain.Views;

public class NotificationRule : IReadOnlyNotificationRule
{
    public object Trigger { get; set; } = null!;
    public object[] Channels { get; set; } = null!;
    public object Schedule { get; set; } = null!;
    public bool IsActive { get; set; }
    public string? TemplateId { get; set; }

    [JsonPropertyName("id")]
    public Guid? Id { get; set; }

    [JsonPropertyName("userId")]
    public string UserId { get; set; } = null!;

    [JsonPropertyName("listId")]
    public Guid ListId { get; set; }
}