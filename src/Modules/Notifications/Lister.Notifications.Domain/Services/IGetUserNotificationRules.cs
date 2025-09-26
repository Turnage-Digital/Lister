using Lister.Notifications.Domain.Views;

namespace Lister.Notifications.Domain.Services;

public interface IGetUserNotificationRules
{
    Task<IEnumerable<NotificationRule>> GetAsync(string userId, Guid? listId, CancellationToken cancellationToken);
}