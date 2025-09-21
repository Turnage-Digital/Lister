using Lister.Notifications.Domain.Entities;
using Lister.Notifications.Domain.Enums;
using Lister.Notifications.Domain.Views;

namespace Lister.Notifications.Domain.Services;

public interface INotificationRuleQueryService
{
    Task<IEnumerable<NotificationRule>> GetByUserAsync(
        string userId,
        Guid? listId = null,
        CancellationToken cancellationToken = default
    );

    Task<IEnumerable<IWritableNotificationRule>> GetActiveRulesForListAsync(
        Guid listId,
        TriggerType? triggerType = null,
        CancellationToken cancellationToken = default
    );
}