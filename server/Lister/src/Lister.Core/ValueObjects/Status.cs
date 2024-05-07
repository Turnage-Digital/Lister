using Newtonsoft.Json;

namespace Lister.Core.ValueObjects;

public record Status
{
    [JsonProperty("name")]
    public string Name { get; set; } = null!;

    [JsonProperty("color")]
    public string Color { get; set; } = null!;
}