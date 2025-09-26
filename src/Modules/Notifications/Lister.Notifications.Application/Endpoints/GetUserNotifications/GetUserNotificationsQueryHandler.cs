using Lister.Notifications.Domain.Entities;
using Lister.Notifications.Domain.Services;
using MediatR;

namespace Lister.Notifications.Application.Endpoints.GetUserNotifications;

public class GetUserNotificationsQueryHandler<TRule, TNotification>(
    INotificationQueryService service
) : IRequestHandler<GetUserNotificationsQuery, GetUserNotificationsResponse>
    where TRule : IWritableNotificationRule
    where TNotification : IWritableNotification
{
    public async Task<GetUserNotificationsResponse> Handle(
        GetUserNotificationsQuery request,
        CancellationToken cancellationToken
    )
    {
        var notifications = await service.GetUserNotificationsAsync(
            request.UserId,
            request.Since,
            request.PageSize,
            request.Page,
            cancellationToken);

        var notificationList = notifications.ToList();
        var filteredNotifications = notificationList.AsEnumerable();

        if (request.Unread.HasValue)
        {
            // if (request.Unread.Value)
            // {
            //     filteredNotifications = filteredNotifications.Where(n => !n.ReadOn.HasValue);
            // }
            // else
            // {
            //     filteredNotifications = filteredNotifications.Where(n => n.ReadOn.HasValue);
            // }
        }

        if (request.ListId.HasValue)
        {
            filteredNotifications = filteredNotifications.Where(n => n.ListId == request.ListId.Value);
        }

        var finalNotifications = filteredNotifications.ToList();

        var unreadCount = await service.GetUnreadCountAsync(
            request.UserId,
            request.ListId,
            cancellationToken);

        return new GetUserNotificationsResponse
        {
            Notifications = finalNotifications.Select(n => new NotificationDto
                {
                    Id = n.Id ?? Guid.Empty,
                    NotificationRuleId = n.NotificationRuleId,
                    UserId = n.UserId,
                    ListId = n.ListId,
                    ItemId = n.ItemId,
                    Title = "Notification", // TODO: Extract from content
                    Body = "Notification body", // TODO: Extract from content  
                    // CreatedOn = n.CreatedOn,
                    // ProcessedOn = n.ProcessedOn,
                    // DeliveredOn = n.DeliveredOn,
                    // ReadOn = n.ReadOn,
                    Metadata = null // TODO: Add metadata from content
                })
                .ToList(),
            TotalCount = finalNotifications.Count,
            UnreadCount = unreadCount,
            HasMore = finalNotifications.Count == request.PageSize
        };
    }
}