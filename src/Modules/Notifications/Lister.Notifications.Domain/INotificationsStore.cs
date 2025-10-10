using Lister.Notifications.Domain.Entities;
using Lister.Notifications.Domain.Enums;
using Lister.Notifications.Domain.ValueObjects;

namespace Lister.Notifications.Domain;

public interface INotificationsStore<TNotification>
    where TNotification : IWritableNotification
{
    Task<TNotification> InitAsync(string userId, Guid listId, CancellationToken cancellationToken);
    Task<TNotification?> GetByIdAsync(Guid id, string userId, CancellationToken cancellationToken);
    Task CreateAsync(TNotification notification, CancellationToken cancellationToken);

    Task SetContentAsync(
        TNotification notification,
        NotificationContent notificationContent,
        CancellationToken cancellationToken
    );

    Task SetPriorityAsync(
        TNotification notification,
        NotificationPriority notificationPriority,
        CancellationToken cancellationToken
    );

    Task SetItemAsync(TNotification notification, int itemId, CancellationToken cancellationToken);
    Task SetRuleAsync(TNotification notification, Guid ruleId, CancellationToken cancellationToken);

    Task MarkAsProcessedAsync(TNotification notification, DateTime processedOn, CancellationToken cancellationToken);
    Task MarkAsDeliveredAsync(TNotification notification, DateTime deliveredOn, CancellationToken cancellationToken);

    Task AddDeliveryAttemptAsync(
        TNotification notification,
        NotificationDeliveryAttempt notificationDeliveryAttempt,
        CancellationToken cancellationToken
    );

    Task<int> GetDeliveryAttemptCountAsync(
        TNotification notification,
        NotificationChannel notificationChannel,
        CancellationToken cancellationToken
    );

    // Task<IEnumerable<TNotification>> GetUserNotificationsAsync(
    //     string userId,
    //     DateTime? since = null,
    //     int pageSize = 20,
    //     int page = 0,
    //     CancellationToken cancellationToken = default
    // );

    // Task<int> GetUnreadCountAsync(
    //     string userId,
    //     Guid? listId = null,
    //     CancellationToken cancellationToken = default
    // );

    Task MarkAsReadAsync(
        TNotification notification,
        DateTime readOn,
        CancellationToken cancellationToken = default
    );

    Task MarkAllAsReadAsync(
        string userId,
        DateTime readOn,
        DateTime? before = null,
        CancellationToken cancellationToken = default
    );
}