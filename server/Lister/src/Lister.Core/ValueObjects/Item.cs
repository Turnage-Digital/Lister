using Newtonsoft.Json;

namespace Lister.Core.ValueObjects;

public record Item
{
    [JsonProperty("bag")]
    public object Bag { get; set; } = null!;

    [JsonProperty("createdBy")]
    public string CreatedBy { get; set; } = null!;

    [JsonProperty("createdOn")]
    public DateTime CreatedOn { get; set; }

    [JsonProperty("id")]
    public int? Id { get; set; }
}