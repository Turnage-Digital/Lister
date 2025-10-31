using Lister.Core.Application;
using Lister.Notifications.ReadOnly.Dtos;

namespace Lister.Notifications.Application.Endpoints.Queries.GetNotificationDetails;

public record GetNotificationDetailsQuery : RequestBase<NotificationDetailsDto?>
{
    public Guid NotificationId { get; set; }
}