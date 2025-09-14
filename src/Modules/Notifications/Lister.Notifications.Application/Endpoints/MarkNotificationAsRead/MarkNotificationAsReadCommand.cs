using MediatR;

namespace Lister.Notifications.Application.Endpoints.MarkNotificationAsRead;

public class MarkNotificationAsReadCommand : IRequest
{
    public Guid NotificationId { get; set; }
    public string UserId { get; set; } = string.Empty;
}