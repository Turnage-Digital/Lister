using Lister.Notifications.Domain.Entities;

namespace Lister.Notifications.Domain.Services;

public interface IGetPendingNotifications
{
    Task<IEnumerable<IWritableNotification>> GetAsync(int batchSize, CancellationToken cancellationToken);
}