using System.Text.Json.Serialization;

namespace Lister.Lists.Domain.ValueObjects;

public record Status
{
    [JsonPropertyName("name")] public string Name { get; set; } = null!;

    [JsonPropertyName("color")] public string Color { get; set; } = null!;
}