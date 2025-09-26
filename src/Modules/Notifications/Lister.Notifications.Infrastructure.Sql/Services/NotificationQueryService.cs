using Lister.Notifications.Domain.Entities;
using Lister.Notifications.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace Lister.Notifications.Infrastructure.Sql.Services;

public class NotificationQueryService(NotificationsDbContext context) : INotificationQueryService
{
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
        int pageSize = 20,
        int page = 0,
        CancellationToken cancellationToken = default
    )
    {
        var query = context.Notifications
            .Where(n => n.UserId == userId);

        if (since.HasValue)
        {
            query = query.Where(n => n.CreatedOn >= since.Value);
        }

        return await query
            .OrderByDescending(n => n.CreatedOn)
            .Skip(page * pageSize)
            .Take(pageSize)
            .Cast<IWritableNotification>()
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetUnreadCountAsync(
        string userId,
        Guid? listId = null,
        CancellationToken cancellationToken = default
    )
    {
        var query = context.Notifications
            .Where(n => n.UserId == userId && n.ReadOn == null);

        if (listId.HasValue)
        {
            query = query.Where(n => n.ListId == listId.Value);
        }

        return await query.CountAsync(cancellationToken);
    }
}