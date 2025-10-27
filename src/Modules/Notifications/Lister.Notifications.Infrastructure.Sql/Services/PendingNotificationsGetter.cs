using System.Linq;
using Lister.Notifications.ReadOnly.Dtos;
using Lister.Notifications.ReadOnly.Queries;
using Microsoft.EntityFrameworkCore;

namespace Lister.Notifications.Infrastructure.Sql.Services;

public class PendingNotificationsGetter(NotificationsDbContext context)
    : IGetPendingNotifications
{
    public async Task<IEnumerable<PendingNotificationDto>> GetAsync(int batchSize, CancellationToken cancellationToken)
    {
        var notifications = await context.Notifications
            .Where(n => n.ProcessedOn == null)
            .OrderBy(n => n.CreatedOn)
            .Take(batchSize)
            .Select(n => new PendingNotificationDto
            {
                Id = n.Id!.Value,
                UserId = n.UserId,
                ListId = n.ListId,
                ItemId = n.ItemId
            })
            .ToListAsync(cancellationToken);

        return notifications;
    }
}
