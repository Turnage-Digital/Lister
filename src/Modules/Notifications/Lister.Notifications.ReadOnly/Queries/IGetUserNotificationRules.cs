using Lister.Notifications.ReadOnly.Dtos;

namespace Lister.Notifications.ReadOnly.Queries;

public interface IGetUserNotificationRules
{
    Task<IEnumerable<NotificationRuleDto>> GetAsync(string userId, Guid? listId, CancellationToken cancellationToken);
}
