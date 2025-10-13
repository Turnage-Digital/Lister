using Lister.Notifications.Domain.Entities;
using Lister.Notifications.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace Lister.Notifications.Infrastructure.Sql.Services;

public class PendingNotificationsGetter(NotificationsDbContext context)
    : IGetPendingNotifications
{
    public async Task<IEnumerable<IWritableNotification>> GetAsync(int batchSize, CancellationToken cancellationToken)
    {
        var retval = await context.Notifications
            .Where(n => n.ProcessedOn == null)
            .OrderBy(n => n.CreatedOn)
            .Take(batchSize)
            .Cast<IWritableNotification>()
            .ToListAsync(cancellationToken);
        return retval;
    }
}