using Lister.Notifications.Domain.Entities;
using Lister.Notifications.Domain.Enums;
using Lister.Notifications.Domain.ValueObjects;

namespace Lister.Notifications.Domain;

public interface INotificationsStore<TNotification>
    where TNotification : IWritableNotification
{
    Task<TNotification> InitAsync(string userId, Guid listId, CancellationToken cancellationToken);
    Task<TNotification?> GetByIdForUpdateAsync(Guid id, CancellationToken cancellationToken);
    Task CreateAsync(TNotification notification, CancellationToken cancellationToken);

    Task SetContentAsync(TNotification notification, NotificationContent content, CancellationToken cancellationToken);

    Task SetPriorityAsync(
        TNotification notification,
        NotificationPriority priority,
        CancellationToken cancellationToken
    );

    Task SetItemAsync(TNotification notification, int itemId, CancellationToken cancellationToken);
    Task SetRuleAsync(TNotification notification, Guid ruleId, CancellationToken cancellationToken);

    Task MarkAsProcessedAsync(TNotification notification, DateTime processedOn, CancellationToken cancellationToken);
    Task MarkAsDeliveredAsync(TNotification notification, DateTime deliveredOn, CancellationToken cancellationToken);

    Task AddDeliveryAttemptAsync(
        TNotification notification,
        NotificationDeliveryAttempt attempt,
        CancellationToken cancellationToken
    );

    Task<int> GetDeliveryAttemptCountAsync(
        TNotification notification,
        NotificationChannel channel,
        CancellationToken cancellationToken
    );
}