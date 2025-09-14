using MediatR;

namespace Lister.Notifications.Application.Endpoints.GetNotificationDetails;

public class GetNotificationDetailsQuery : IRequest<NotificationDetailsDto?>
{
    public Guid NotificationId { get; set; }
    public string UserId { get; set; } = string.Empty;
}