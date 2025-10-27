using System.Linq;
using Lister.Notifications.ReadOnly.Queries;
using Microsoft.EntityFrameworkCore;

namespace Lister.Notifications.Infrastructure.Sql.Services;

public class UnreadNotificationCountGetter(NotificationsDbContext context)
    : IGetUnreadNotificationCount
{
    public async Task<int> GetAsync(string userId, Guid? listId, CancellationToken cancellationToken)
    {
        var query = context.Notifications
            .Where(n => n.UserId == userId && n.ReadOn == null);
        if (listId.HasValue)
        {
            query = query.Where(n => n.ListId == listId.Value);
        }

        var retval = await query.CountAsync(cancellationToken);
        return retval;
    }
}
