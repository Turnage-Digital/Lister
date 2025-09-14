namespace Lister.Notifications.Application.Endpoints.GetUserNotifications;

public class NotificationDto
{
    public Guid Id { get; set; }
    public Guid? NotificationRuleId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public Guid ListId { get; set; }
    public int? ItemId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; }
    public DateTime? ProcessedOn { get; set; }
    public DateTime? DeliveredOn { get; set; }
    public DateTime? ReadOn { get; set; }
    public bool IsRead => ReadOn.HasValue;
    public object? Metadata { get; set; }
}