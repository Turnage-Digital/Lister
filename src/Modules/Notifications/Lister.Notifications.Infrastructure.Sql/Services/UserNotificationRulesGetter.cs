using System.Text.Json;
using Lister.Notifications.Domain.Queries;
using Lister.Notifications.Domain.ValueObjects;
using Lister.Notifications.Domain.Views;
using Microsoft.EntityFrameworkCore;

namespace Lister.Notifications.Infrastructure.Sql.Services;

public class UserNotificationRulesGetter(NotificationsDbContext context)
    : IGetUserNotificationRules
{
    public async Task<IEnumerable<NotificationRule>> GetAsync(
        string userId,
        Guid? listId,
        CancellationToken cancellationToken
    )
    {
        var query = context.NotificationRules
            .Where(r => r.UserId == userId && !r.IsDeleted);
        if (listId.HasValue)
        {
            query = query.Where(r => r.ListId == listId.Value);
        }

        var rules = await query.ToListAsync(cancellationToken);
        var retval = rules.Select(rule => new NotificationRule
        {
            Id = rule.Id,
            UserId = rule.UserId,
            ListId = rule.ListId,
            IsActive = rule.IsActive,
            TemplateId = rule.TemplateId,
            Trigger = JsonSerializer.Deserialize<NotificationTrigger>(rule.TriggerJson)!,
            Channels = JsonSerializer.Deserialize<NotificationChannel[]>(rule.ChannelsJson)!,
            Schedule = JsonSerializer.Deserialize<NotificationSchedule>(rule.ScheduleJson)!
        });
        return retval;
    }
}