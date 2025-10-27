using System.Text.Json.Serialization;
using Lister.Notifications.Domain.Enums;

namespace Lister.Notifications.ReadOnly.Dtos;

public record NotificationScheduleDto
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
}
