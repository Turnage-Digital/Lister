using Lister.Notifications.Domain.Entities;
using Lister.Notifications.Domain.Enums;
using Lister.Notifications.Domain.ValueObjects;

namespace Lister.Notifications.Domain;

public interface INotificationRulesStore<TRule>
    where TRule : IWritableNotificationRule
{
    Task<TRule> InitAsync(string userId, Guid listId, CancellationToken cancellationToken);
    Task<TRule?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<TRule>> GetByUserAsync(string userId, Guid? listId, CancellationToken cancellationToken);

    Task<IEnumerable<TRule>> GetActiveRulesAsync(
        Guid listId,
        TriggerType? triggerType,
        CancellationToken cancellationToken
    );

    Task CreateAsync(TRule rule, CancellationToken cancellationToken);
    Task UpdateAsync(TRule rule, string updatedBy, CancellationToken cancellationToken);
    Task DeleteAsync(TRule rule, string deletedBy, CancellationToken cancellationToken);

    Task SetTriggerAsync(TRule rule, NotificationTrigger trigger, CancellationToken cancellationToken);
    Task<NotificationTrigger> GetTriggerAsync(TRule rule, CancellationToken cancellationToken);

    Task SetChannelsAsync(TRule rule, NotificationChannel[] channels, CancellationToken cancellationToken);
    Task<NotificationChannel[]> GetChannelsAsync(TRule rule, CancellationToken cancellationToken);

    Task SetScheduleAsync(TRule rule, NotificationSchedule schedule, CancellationToken cancellationToken);
    Task<NotificationSchedule> GetScheduleAsync(TRule rule, CancellationToken cancellationToken);

    Task SetTemplateAsync(TRule rule, string templateId, CancellationToken cancellationToken);
    Task SetActiveStatusAsync(TRule rule, bool isActive, CancellationToken cancellationToken);
}