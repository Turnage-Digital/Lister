using System.Text.Json.Serialization;
using Lister.Notifications.Domain.Enums;

namespace Lister.Notifications.Domain.ValueObjects;

public record NotificationChannel
{
    [JsonPropertyName("type")]
    public ChannelType Type { get; init; }

    [JsonPropertyName("address")]
    public string? Address { get; init; }

    [JsonPropertyName("settings")]
    public Dictionary<string, string> Settings { get; init; } = new();

    public static NotificationChannel Email(string emailAddress)
    {
        return new NotificationChannel
        {
            Type = ChannelType.Email,
            Address = emailAddress
        };
    }

    public static NotificationChannel Sms(string phoneNumber)
    {
        return new NotificationChannel
        {
            Type = ChannelType.Sms,
            Address = phoneNumber
        };
    }

    public static NotificationChannel InApp()
    {
        return new NotificationChannel { Type = ChannelType.InApp };
    }

    public static NotificationChannel Push(string deviceToken)
    {
        return new NotificationChannel
        {
            Type = ChannelType.Push,
            Address = deviceToken
        };
    }

    public static NotificationChannel Webhook(string url)
    {
        return new NotificationChannel
        {
            Type = ChannelType.Webhook,
            Address = url
        };
    }
}