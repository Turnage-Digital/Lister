using Lister.Notifications.Domain.Entities;
using Lister.Notifications.Domain.Enums;
using Lister.Notifications.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace Lister.Notifications.Infrastructure.Sql.Services;

public class ActiveNotificationRulesGetter(NotificationsDbContext context) 
    : IGetActiveNotificationRules
{
    public async Task<IEnumerable<IWritableNotificationRule>> GetAsync(
        Guid listId,
        TriggerType? triggerType,
        CancellationToken cancellationToken
    )
    {
        var query = context.NotificationRules
            .Where(r => r.ListId == listId && r.IsActive && !r.IsDeleted);
        if (triggerType.HasValue)
        {
            var tt = (int)triggerType.Value;
            query = query.Where(r => r.TriggerType == tt);
        }

        var retval = await query.Cast<IWritableNotificationRule>()
            .ToListAsync(cancellationToken);
        return retval;
    }
}