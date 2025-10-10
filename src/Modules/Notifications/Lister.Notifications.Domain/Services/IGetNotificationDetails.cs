using Lister.Notifications.Domain.Views;

namespace Lister.Notifications.Domain.Services;

public interface IGetNotificationDetails
{
    Task<NotificationDetails?> GetAsync(string userId, Guid notificationId, CancellationToken cancellationToken);
}