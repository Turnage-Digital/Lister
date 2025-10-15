using Lister.Notifications.Domain.Views;

namespace Lister.Notifications.Domain.Queries;

public interface IGetNotificationDetails
{
    Task<NotificationDetails?> GetAsync(string userId, Guid notificationId, CancellationToken cancellationToken);
}