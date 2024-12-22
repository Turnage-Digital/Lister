using System.Text.Json.Serialization;

namespace Lister.Core.Domain.ValueObjects;

public record Entry<TType>
    where TType : Enum
{
    [JsonPropertyName("type")]
    public TType Type { get; set; } = default!;

    [JsonPropertyName("on")]
    public DateTime On { get; set; }

    [JsonPropertyName("by")]
    public string By { get; set; } = null!;

    [JsonPropertyName("bag")]
    public object? Bag { get; set; }
}