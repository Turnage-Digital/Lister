using Lister.Core.Application;

namespace Lister.Notifications.Application.Commands.MarkNotificationAsRead;

public record MarkNotificationAsReadCommand : RequestBase
{
    public Guid NotificationId { get; set; }
}