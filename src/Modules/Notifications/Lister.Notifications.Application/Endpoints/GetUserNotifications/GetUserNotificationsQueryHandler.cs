using Lister.Notifications.Domain.Entities;
using Lister.Notifications.Domain.Queries;
using Lister.Notifications.Domain.Views;
using MediatR;

namespace Lister.Notifications.Application.Endpoints.GetUserNotifications;

public class GetUserNotificationsQueryHandler<TRule, TNotification>(
    IGetUserNotifications getter
) : IRequestHandler<GetUserNotificationsQuery, NotificationListPage>
    where TRule : IWritableNotificationRule
    where TNotification : IWritableNotification
{
    public async Task<NotificationListPage> Handle(
        GetUserNotificationsQuery request,
        CancellationToken cancellationToken
    )
    {
        var page = await getter.GetAsync(
            request.UserId!,
            request.Since,
            request.ListId,
            request.Unread,
            request.PageSize,
            request.Page,
            cancellationToken);
        return page;
    }
}