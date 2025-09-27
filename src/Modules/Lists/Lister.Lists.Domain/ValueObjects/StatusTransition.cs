using System.Text.Json.Serialization;

namespace Lister.Lists.Domain.ValueObjects;

public record StatusTransition
{
    [JsonPropertyName("from")]
    public string From { get; init; } = string.Empty;

    [JsonPropertyName("to")]
    public string[] AllowedNext { get; init; } = [];
}