using Lister.Notifications.ReadOnly.Dtos;

namespace Lister.Notifications.ReadOnly.Queries;

public interface IGetNotificationDetails
{
    Task<NotificationDetailsDto?> GetAsync(string userId, Guid notificationId, CancellationToken cancellationToken);
}
