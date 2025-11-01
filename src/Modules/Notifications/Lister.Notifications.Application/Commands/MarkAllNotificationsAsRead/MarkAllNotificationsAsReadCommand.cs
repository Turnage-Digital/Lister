using Lister.Core.Application;

namespace Lister.Notifications.Application.Commands.MarkAllNotificationsAsRead;

public record MarkAllNotificationsAsReadCommand : RequestBase
{
    public DateTime? Before { get; set; }
}