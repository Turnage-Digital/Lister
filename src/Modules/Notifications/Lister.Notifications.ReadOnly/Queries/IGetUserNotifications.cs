using Lister.Notifications.ReadOnly.Dtos;

namespace Lister.Notifications.ReadOnly.Queries;

public interface IGetUserNotifications
{
    Task<NotificationListPageDto> GetAsync(
        string userId,
        DateTime? since,
        Guid? listId,
        bool? unread,
        int pageSize,
        int page,
        CancellationToken cancellationToken
    );
}