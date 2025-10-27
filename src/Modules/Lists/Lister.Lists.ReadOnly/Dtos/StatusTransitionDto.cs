using System.Text.Json.Serialization;

namespace Lister.Lists.ReadOnly.Dtos;

public record StatusTransitionDto
{
    [JsonPropertyName("from")]
    public string From { get; init; } = string.Empty;

    [JsonPropertyName("to")]
    public string[] AllowedNext { get; init; } = [];
}
