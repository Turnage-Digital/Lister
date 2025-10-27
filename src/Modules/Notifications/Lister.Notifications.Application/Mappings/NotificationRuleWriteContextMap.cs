using System.Collections.Generic;
using System.Linq;
using Lister.Notifications.Domain;
using Lister.Notifications.Domain.ValueObjects;
using Lister.Notifications.ReadOnly.Dtos;

namespace Lister.Notifications.Application.Mappings;

internal static class NotificationRuleWriteContextMap
{
    public static NotificationRuleDto ToDto(
        INotificationRule rule,
        NotificationTrigger trigger,
        NotificationChannel[] channels,
        NotificationSchedule schedule,
        string? templateId,
        bool isActive
    ) =>
        new()
        {
            Id = rule.Id,
            UserId = rule.UserId,
            ListId = rule.ListId,
            TemplateId = templateId,
            IsActive = isActive,
            Trigger = ToDto(trigger),
            Channels = channels.Select(ToDto).ToArray(),
            Schedule = ToDto(schedule)
        };

    private static NotificationTriggerDto ToDto(NotificationTrigger trigger) =>
        new()
        {
            Type = trigger.Type,
            FromValue = trigger.FromValue,
            ToValue = trigger.ToValue,
            ColumnName = trigger.ColumnName,
            Operator = trigger.Operator,
            Value = trigger.Value
        };

    private static NotificationChannelDto ToDto(NotificationChannel channel) =>
        new()
        {
            Type = channel.Type,
            Address = channel.Address,
            Settings = new Dictionary<string, string>(channel.Settings)
        };

    private static NotificationScheduleDto ToDto(NotificationSchedule schedule) =>
        new()
        {
            Type = schedule.Type,
            Delay = schedule.Delay,
            CronExpression = schedule.CronExpression,
            DailyAt = schedule.DailyAt,
            DaysOfWeek = schedule.DaysOfWeek
        };
}
