using Lister.Notifications.Domain.Entities;
using Lister.Notifications.Domain.Enums;

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
        DeliveryStatus? status = null,
        CancellationToken cancellationToken = default
    );
}