using Lister.Notifications.Domain.Enums;
using Lister.Notifications.ReadOnly.Dtos;

namespace Lister.Notifications.ReadOnly.Queries;

public interface IGetActiveNotificationRules
{
    Task<IEnumerable<NotificationRuleDto>> GetAsync(
        Guid listId,
        IReadOnlyCollection<TriggerType>? triggerTypes,
        CancellationToken cancellationToken
    );
}