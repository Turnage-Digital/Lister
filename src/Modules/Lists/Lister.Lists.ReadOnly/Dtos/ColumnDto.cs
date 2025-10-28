using System.Text.Json.Serialization;
using Lister.Lists.Domain.Enums;

namespace Lister.Lists.ReadOnly.Dtos;

public record ColumnDto
{
    [JsonPropertyName("key")]
    public string? StorageKey { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("property")]
    public string Property { get; init; } = string.Empty;

    [JsonPropertyName("type")]
    public ColumnType Type { get; init; }

    [JsonPropertyName("required")]
    public bool Required { get; init; }

    [JsonPropertyName("allowedValues")]
    public string[]? AllowedValues { get; init; }

    [JsonPropertyName("minNumber")]
    public decimal? MinNumber { get; init; }

    [JsonPropertyName("maxNumber")]
    public decimal? MaxNumber { get; init; }

    [JsonPropertyName("regex")]
    public string? Regex { get; init; }
}