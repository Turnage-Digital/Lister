using Lister.Core.Application;

namespace Lister.Notifications.Application.Queries.GetUnreadNotificationCount;

public record GetUnreadNotificationCountQuery : RequestBase<int>
{
    public Guid? ListId { get; set; }
}