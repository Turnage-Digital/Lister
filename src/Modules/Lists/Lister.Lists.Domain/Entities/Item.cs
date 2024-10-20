using System.Text.Json.Serialization;

namespace Lister.Lists.Domain.Entities;

public class Item
{
    [JsonPropertyName("bag")]
    public object Bag { get; set; } = null!;

    [JsonPropertyName("createdBy")]
    public string CreatedBy { get; set; } = null!;

    [JsonPropertyName("createdOn")]
    public DateTime CreatedOn { get; set; }

    [JsonPropertyName("id")]
    public int? Id { get; set; }
}