using System.Text.Json;
using Lister.Notifications.Domain.Entities;
using Lister.Notifications.Domain.Enums;
using Lister.Notifications.Domain.Services;
using Lister.Notifications.Domain.Views;
using Microsoft.EntityFrameworkCore;

namespace Lister.Notifications.Infrastructure.Sql.Services;

public class NotificationRuleQueryService(NotificationsDbContext context) : INotificationRuleQueryService
{
    public async Task<IWritableNotificationRule?> GetByIdForUpdateAsync(
        Guid id,
        CancellationToken cancellationToken = default
    )
    {
        return await context.NotificationRules
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted, cancellationToken);
    }

    public async Task<IEnumerable<NotificationRule>> GetByUserAsync(
        string userId,
        Guid? listId = null,
        CancellationToken cancellationToken = default
    )
    {
        var query = context.NotificationRules
            .Where(r => r.UserId == userId && !r.IsDeleted);

        if (listId.HasValue)
        {
            query = query.Where(r => r.ListId == listId.Value);
        }

        var rules = await query.ToListAsync(cancellationToken);

        return rules.Select(rule => new NotificationRule
        {
            Id = rule.Id,
            UserId = rule.UserId,
            ListId = rule.ListId,
            IsActive = rule.IsActive,
            TemplateId = rule.TemplateId,
            CreatedOn = rule.CreatedOn,
            CreatedBy = rule.CreatedBy,
            UpdatedOn = rule.UpdatedOn,
            UpdatedBy = rule.UpdatedBy,
            TriggerJson = rule.TriggerJson,
            ChannelsJson = rule.ChannelsJson,
            ScheduleJson = rule.ScheduleJson,
            Trigger = JsonSerializer.Deserialize<object>(rule.TriggerJson)!,
            Channels = JsonSerializer.Deserialize<object[]>(rule.ChannelsJson)!,
            Schedule = JsonSerializer.Deserialize<object>(rule.ScheduleJson)!
        });
    }

    public async Task<IEnumerable<IWritableNotificationRule>> GetActiveRulesForListAsync(
        Guid listId,
        TriggerType? triggerType = null,
        CancellationToken cancellationToken = default
    )
    {
        var query = context.NotificationRules
            .Where(r => r.ListId == listId && r.IsActive && !r.IsDeleted);

        if (triggerType.HasValue)
        {
            // Simple trigger type filtering - in real implementation you'd deserialize and check
            var triggerTypeStr = triggerType.Value.ToString();
            query = query.Where(r => r.TriggerJson.Contains(triggerTypeStr));
        }

        return await query.Cast<IWritableNotificationRule>().ToListAsync(cancellationToken);
    }
}