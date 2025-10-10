using Lister.Core.Application;

namespace Lister.Notifications.Application.Endpoints.MarkAllNotificationsAsRead;

public record MarkAllNotificationsAsReadCommand : RequestBase
{
    public DateTime? Before { get; set; }
}