using Lister.Core.Application;

namespace Lister.Notifications.Application.Endpoints.Commands.MarkNotificationAsRead;

public record MarkNotificationAsReadCommand : RequestBase
{
    public Guid NotificationId { get; set; }
}