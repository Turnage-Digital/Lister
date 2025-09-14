using MediatR;

namespace Lister.Notifications.Application.Endpoints.MarkAllNotificationsAsRead;

public class MarkAllNotificationsAsReadCommand : IRequest
{
    public string UserId { get; set; } = string.Empty;
    public DateTime? Before { get; set; }
}