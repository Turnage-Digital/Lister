using Lister.Notifications.Domain.Entities;

namespace Lister.Notifications.Domain.Queries;

public interface IGetPendingNotifications
{
    Task<IEnumerable<IWritableNotification>> GetAsync(int batchSize, CancellationToken cancellationToken);
}