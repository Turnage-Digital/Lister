using Lister.Notifications.Domain.Entities;
using Lister.Notifications.Domain.Enums;
using Lister.Notifications.Domain.Queries;
using Microsoft.EntityFrameworkCore;

namespace Lister.Notifications.Infrastructure.Sql.Services;

public class ActiveNotificationRulesGetter(NotificationsDbContext context)
    : IGetActiveNotificationRules
{
    public async Task<IEnumerable<IWritableNotificationRule>> GetAsync(
        Guid listId,
        IReadOnlyCollection<TriggerType>? triggerTypes,
        CancellationToken cancellationToken
    )
    {
        var query = context.NotificationRules
            .Where(r => r.ListId == listId && r.IsActive && !r.IsDeleted);
        if (triggerTypes is { Count: > 0 })
        {
            var triggerValues = triggerTypes
                .Select(t => (int)t)
                .ToArray();
            query = query.Where(r => triggerValues.Contains(r.TriggerType));
        }

        var retval = await query.Cast<IWritableNotificationRule>()
            .ToListAsync(cancellationToken);
        return retval;
    }
}