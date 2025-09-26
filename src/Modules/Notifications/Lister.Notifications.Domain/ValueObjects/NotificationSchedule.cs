using System.Text.Json.Serialization;
using Lister.Notifications.Domain.Enums;

namespace Lister.Notifications.Domain.ValueObjects;

public record NotificationSchedule
{
    [JsonPropertyName("type")]
    public ScheduleType Type { get; init; }

    [JsonPropertyName("delay")]
    public TimeSpan? Delay { get; init; }

    [JsonPropertyName("cronExpression")]
    public string? CronExpression { get; init; }

    [JsonPropertyName("dailyAt")]
    public TimeOnly? DailyAt { get; init; }

    [JsonPropertyName("daysOfWeek")]
    public DayOfWeek[]? DaysOfWeek { get; init; }

    // Factory methods
    public static NotificationSchedule Immediate()
    {
        return new NotificationSchedule { Type = ScheduleType.Immediate };
    }

    public static NotificationSchedule Delayed(TimeSpan delay)
    {
        return new NotificationSchedule
        {
            Type = ScheduleType.Delayed,
            Delay = delay
        };
    }

    public static NotificationSchedule Daily(TimeOnly at)
    {
        return new NotificationSchedule
        {
            Type = ScheduleType.Daily,
            DailyAt = at
        };
    }

    public static NotificationSchedule Batched(TimeSpan batchWindow)
    {
        return new NotificationSchedule
        {
            Type = ScheduleType.Batched,
            Delay = batchWindow
        };
    }

    public static NotificationSchedule Weekly(DayOfWeek[] days, TimeOnly at)
    {
        return new NotificationSchedule
        {
            Type = ScheduleType.Weekly,
            DaysOfWeek = days,
            DailyAt = at
        };
    }
}