using Lister.Core.Application;
using Lister.Notifications.ReadOnly.Dtos;

namespace Lister.Notifications.Application.Endpoints.GetUserNotifications;

public record GetUserNotificationsQuery : RequestBase<NotificationListPageDto>
{
    public Guid? ListId { get; set; }
    public DateTime? Since { get; set; }
    public bool? Unread { get; set; }
    public int PageSize { get; set; } = 20;
    public int Page { get; set; } = 0;
}
