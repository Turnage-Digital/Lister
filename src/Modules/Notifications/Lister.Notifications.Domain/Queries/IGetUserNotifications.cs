using Lister.Notifications.Domain.Views;

namespace Lister.Notifications.Domain.Queries;

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