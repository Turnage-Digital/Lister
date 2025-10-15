using Lister.Notifications.Domain.Entities;
using Lister.Notifications.Domain.Enums;

namespace Lister.Notifications.Domain.Queries;

public interface IGetActiveNotificationRules
{
    Task<IEnumerable<IWritableNotificationRule>> GetAsync(
        Guid listId,
        IReadOnlyCollection<TriggerType>? triggerTypes,
        CancellationToken cancellationToken
    );
}