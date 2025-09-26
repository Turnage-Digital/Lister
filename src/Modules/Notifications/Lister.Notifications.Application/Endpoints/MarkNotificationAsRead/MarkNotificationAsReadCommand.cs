using Lister.Core.Application;

namespace Lister.Notifications.Application.Endpoints.MarkNotificationAsRead;

public record MarkNotificationAsReadCommand : RequestBase
{
    public Guid NotificationId { get; set; }
}