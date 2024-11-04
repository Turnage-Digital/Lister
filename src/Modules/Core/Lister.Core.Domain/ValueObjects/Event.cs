using System.Text.Json.Serialization;

namespace Lister.Core.Domain.ValueObjects;

public record Event
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = null!;

    [JsonPropertyName("on")]
    public DateTime On { get; set; }

    [JsonPropertyName("by")]
    public string By { get; set; } = null!;
}