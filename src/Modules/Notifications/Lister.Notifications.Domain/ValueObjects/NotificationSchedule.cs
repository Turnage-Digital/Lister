using Lister.Notifications.Domain.Enums;

namespace Lister.Notifications.Domain.ValueObjects;

public record NotificationSchedule
{
    public ScheduleType Type { get; init; }
    public TimeSpan? Delay { get; init; }
    public string? CronExpression { get; init; }
    public TimeOnly? DailyAt { get; init; }
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