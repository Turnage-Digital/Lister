using Lister.Notifications.Domain.Entities;

namespace Lister.Notifications.Domain.Services;

public interface INotificationQueryService
{
    Task<IEnumerable<IWritableNotification>> GetPendingNotificationsAsync(
        int batchSize = 100,
        CancellationToken cancellationToken = default
    );

    Task<IEnumerable<IWritableNotification>> GetUserNotificationsAsync(
        string userId,
        DateTime? since = null,
        int pageSize = 20,
        int page = 0,
        CancellationToken cancellationToken = default
    );

    Task<int> GetUnreadCountAsync(
        string userId,
        Guid? listId = null,
        CancellationToken cancellationToken = default
    );
}