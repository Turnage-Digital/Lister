using Lister.Notifications.Domain.Entities;
using Lister.Notifications.Domain.Enums;
using Lister.Notifications.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace Lister.Notifications.Infrastructure.Sql.Services;

public class NotificationQueryService(NotificationsDbContext context) : INotificationQueryService
{
    public async Task<IWritableNotification?> GetByIdForUpdateAsync(
        Guid id,
        CancellationToken cancellationToken = default
    )
    {
        return await context.Notifications
            .FirstOrDefaultAsync(n => n.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<IWritableNotification>> GetPendingNotificationsAsync(
        int batchSize = 100,
        CancellationToken cancellationToken = default
    )
    {
        return await context.Notifications
            .Where(n => n.ProcessedOn == null)
            .OrderBy(n => n.CreatedOn)
            .Take(batchSize)
            .Cast<IWritableNotification>()
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<IWritableNotification>> GetUserNotificationsAsync(
        string userId,
        DateTime? since = null,
        DeliveryStatus? status = null,
        CancellationToken cancellationToken = default
    )
    {
        var query = context.Notifications
            .Where(n => n.UserId == userId);

        if (since.HasValue)
        {
            query = query.Where(n => n.CreatedOn >= since.Value);
        }

        if (status.HasValue)
        {
            if (status == DeliveryStatus.Delivered)
            {
                query = query.Where(n => n.DeliveredOn != null);
            }
            else if (status == DeliveryStatus.Failed)
            {
                query = query.Where(n => n.DeliveredOn == null && n.ProcessedOn != null);
            }
            else // Pending
            {
                query = query.Where(n => n.ProcessedOn == null);
            }
        }

        return await query
            .OrderByDescending(n => n.CreatedOn)
            .Cast<IWritableNotification>()
            .ToListAsync(cancellationToken);
    }
}