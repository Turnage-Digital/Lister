using System.Text.Json.Serialization;
using Lister.Notifications.Domain.Enums;

namespace Lister.Notifications.ReadOnly.Dtos;

public record NotificationTriggerDto
{
    [JsonPropertyName("type")]
    public TriggerType Type { get; init; }

    [JsonPropertyName("fromValue")]
    public string? FromValue { get; init; }

    [JsonPropertyName("toValue")]
    public string? ToValue { get; init; }

    [JsonPropertyName("columnName")]
    public string? ColumnName { get; init; }

    [JsonPropertyName("operator")]
    public string? Operator { get; init; }

    [JsonPropertyName("value")]
    public string? Value { get; init; }
}
