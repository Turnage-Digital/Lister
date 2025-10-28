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
    )
    {
        return new NotificationRuleDto
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
    }

    private static NotificationTriggerDto ToDto(NotificationTrigger trigger)
    {
        return new NotificationTriggerDto
        {
            Type = trigger.Type,
            FromValue = trigger.FromValue,
            ToValue = trigger.ToValue,
            ColumnName = trigger.ColumnName,
            Operator = trigger.Operator,
            Value = trigger.Value
        };
    }

    private static NotificationChannelDto ToDto(NotificationChannel channel)
    {
        return new NotificationChannelDto
        {
            Type = channel.Type,
            Address = channel.Address,
            Settings = new Dictionary<string, string>(channel.Settings)
        };
    }

    private static NotificationScheduleDto ToDto(NotificationSchedule schedule)
    {
        return new NotificationScheduleDto
        {
            Type = schedule.Type,
            Delay = schedule.Delay,
            CronExpression = schedule.CronExpression,
            DailyAt = schedule.DailyAt,
            DaysOfWeek = schedule.DaysOfWeek
        };
    }
}