using System.Text.Json.Serialization;
using Lister.Core.Domain.ValueObjects;
using Lister.Notifications.Domain.Enums;

namespace Lister.Notifications.ReadOnly.Dtos;

public record NotificationDetailsDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; init; }

    [JsonPropertyName("notificationRuleId")]
    public Guid? NotificationRuleId { get; init; }

    [JsonPropertyName("userId")]
    public string UserId { get; init; } = string.Empty;

    [JsonPropertyName("listId")]
    public Guid ListId { get; init; }

    [JsonPropertyName("itemId")]
    public int? ItemId { get; init; }

    [JsonPropertyName("title")]
    public string Title { get; init; } = string.Empty;

    [JsonPropertyName("body")]
    public string Body { get; init; } = string.Empty;

    [JsonPropertyName("isRead")]
    public bool IsRead { get; init; }

    [JsonPropertyName("metadata")]
    public object? Metadata { get; init; }

    [JsonPropertyName("history")]
    public Entry<NotificationHistoryType>[] History { get; init; } = [];

    [JsonPropertyName("deliveryAttempts")]
    public List<DeliveryAttemptDto> DeliveryAttempts { get; init; } = new();
}

public record DeliveryAttemptDto
{
    [JsonPropertyName("channel")]
    public string Channel { get; init; } = string.Empty;

    [JsonPropertyName("attemptedOn")]
    public DateTime AttemptedOn { get; init; }

    [JsonPropertyName("status")]
    public string Status { get; init; } = string.Empty;

    [JsonPropertyName("failureReason")]
    public string? FailureReason { get; init; }

    [JsonPropertyName("attemptNumber")]
    public int AttemptNumber { get; init; }
}
