using Lister.Notifications.Domain.Views;

namespace Lister.Notifications.Domain.Queries;

public interface IGetUserNotificationRules
{
    Task<IEnumerable<NotificationRule>> GetAsync(string userId, Guid? listId, CancellationToken cancellationToken);
}