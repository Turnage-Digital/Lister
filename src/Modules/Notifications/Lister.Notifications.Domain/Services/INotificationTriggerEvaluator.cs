using Lister.Notifications.Domain.Entities;
using Lister.Notifications.Domain.ValueObjects;

namespace Lister.Notifications.Domain.Services;

public interface INotificationTriggerEvaluator
{
    Task<bool> ShouldTriggerAsync<TNotificationRule>(
        INotificationRulesStore<TNotificationRule> rulesStore,
        TNotificationRule rule,
        NotificationTrigger actualTrigger,
        Dictionary<string, object> context,
        CancellationToken cancellationToken
    ) where TNotificationRule : IWritableNotificationRule;
}