namespace Lister.Notifications.Application.Endpoints.GetUserNotifications;

public class GetUserNotificationsResponse
{
    public List<NotificationDto> Notifications { get; set; } = new();
    public int TotalCount { get; set; }
    public int UnreadCount { get; set; }
    public bool HasMore { get; set; }
}