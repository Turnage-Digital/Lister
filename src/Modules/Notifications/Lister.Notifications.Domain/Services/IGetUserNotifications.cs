using Lister.Notifications.Domain.Views;

namespace Lister.Notifications.Domain.Services;

public interface IGetUserNotifications
{
    Task<NotificationListPage> GetAsync(
        string userId,
        DateTime? since,
        Guid? listId,
        bool? unread,
        int pageSize,
        int page,
        CancellationToken cancellationToken
    );
}