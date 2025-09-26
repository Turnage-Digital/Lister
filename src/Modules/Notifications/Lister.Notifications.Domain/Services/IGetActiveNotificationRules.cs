using Lister.Notifications.Domain.Entities;
using Lister.Notifications.Domain.Enums;

namespace Lister.Notifications.Domain.Services;

public interface IGetActiveNotificationRules
{
    Task<IEnumerable<IWritableNotificationRule>> GetAsync(
        Guid listId,
        TriggerType? triggerType,
        CancellationToken cancellationToken
    );
}