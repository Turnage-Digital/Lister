using Lister.Notifications.ReadOnly.Dtos;

namespace Lister.Notifications.ReadOnly.Queries;

public interface IGetPendingNotifications
{
    Task<IEnumerable<PendingNotificationDto>> GetAsync(int batchSize, CancellationToken cancellationToken);
}