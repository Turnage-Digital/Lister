using System.Text.Json;
using Lister.Notifications.Domain.ValueObjects;
using Lister.Notifications.ReadOnly.Dtos;
using Lister.Notifications.ReadOnly.Queries;
using Microsoft.EntityFrameworkCore;

namespace Lister.Notifications.Infrastructure.Sql.Services;

public class UserNotificationRulesGetter(NotificationsDbContext context)
    : IGetUserNotificationRules
{
    public async Task<IEnumerable<NotificationRuleDto>> GetAsync(
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
        var retval = rules.Select(rule =>
        {
            var trigger = JsonSerializer.Deserialize<NotificationTrigger>(rule.TriggerJson)!;
            var channels = JsonSerializer.Deserialize<NotificationChannel[]>(rule.ChannelsJson)!;
            var schedule = JsonSerializer.Deserialize<NotificationSchedule>(rule.ScheduleJson)!;

            return new NotificationRuleDto
            {
                Id = rule.Id,
                UserId = rule.UserId,
                ListId = rule.ListId,
                IsActive = rule.IsActive,
                TemplateId = rule.TemplateId,
                Trigger = new NotificationTriggerDto
                {
                    Type = trigger.Type,
                    FromValue = trigger.FromValue,
                    ToValue = trigger.ToValue,
                    ColumnName = trigger.ColumnName,
                    Operator = trigger.Operator,
                    Value = trigger.Value
                },
                Channels = channels
                    .Select(channel => new NotificationChannelDto
                    {
                        Type = channel.Type,
                        Address = channel.Address,
                        Settings = channel.Settings
                    })
                    .ToArray(),
                Schedule = new NotificationScheduleDto
                {
                    Type = schedule.Type,
                    Delay = schedule.Delay,
                    CronExpression = schedule.CronExpression,
                    DailyAt = schedule.DailyAt,
                    DaysOfWeek = schedule.DaysOfWeek
                }
            };
        });
        return retval;
    }
}