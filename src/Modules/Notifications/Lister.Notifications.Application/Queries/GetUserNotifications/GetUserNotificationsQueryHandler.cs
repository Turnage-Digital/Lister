using Lister.Notifications.Domain.Entities;
using Lister.Notifications.ReadOnly.Dtos;
using Lister.Notifications.ReadOnly.Queries;
using MediatR;

namespace Lister.Notifications.Application.Queries.GetUserNotifications;

public class GetUserNotificationsQueryHandler<TRule, TNotification>(
    IGetUserNotifications getter
) : IRequestHandler<GetUserNotificationsQuery, NotificationListPageDto>
    where TRule : IWritableNotificationRule
    where TNotification : IWritableNotification
{
    public async Task<NotificationListPageDto> Handle(
        GetUserNotificationsQuery request,
        CancellationToken cancellationToken
    )
    {
        var retval = await getter.GetAsync(
            request.UserId!,
            request.Since,
            request.ListId,
            request.Unread,
            request.PageSize,
            request.Page,
            cancellationToken);
        return retval;
    }
}