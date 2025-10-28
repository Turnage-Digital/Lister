using System.Text.Json.Serialization;

namespace Lister.Lists.ReadOnly.Dtos;

public record StatusDto
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("color")]
    public string Color { get; init; } = string.Empty;
}