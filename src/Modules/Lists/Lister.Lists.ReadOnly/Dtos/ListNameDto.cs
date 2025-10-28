using System.Text.Json.Serialization;

namespace Lister.Lists.ReadOnly.Dtos;

public record ListNameDto
{
    [JsonPropertyName("count")]
    public int Count { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("id")]
    public Guid? Id { get; init; }
}