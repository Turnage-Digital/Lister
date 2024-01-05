using Newtonsoft.Json;

namespace Lister.Core.ValueObjects;

public record Item
{
    [JsonProperty("bag")]
    public object Bag { get; set; } = null!;

    [JsonProperty("id")]
    public Guid? Id { get; set; }
}