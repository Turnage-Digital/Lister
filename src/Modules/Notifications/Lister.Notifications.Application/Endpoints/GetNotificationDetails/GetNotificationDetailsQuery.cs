using Lister.Core.Application;
using Lister.Notifications.Domain.Views;

namespace Lister.Notifications.Application.Endpoints.GetNotificationDetails;

public record GetNotificationDetailsQuery : RequestBase<NotificationDetails?>
{
    public Guid NotificationId { get; set; }
}