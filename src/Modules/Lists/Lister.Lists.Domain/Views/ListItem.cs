using System.Text.Json.Serialization;

namespace Lister.Lists.Domain.Views;

public record ListItem : IReadOnlyItem
{
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    [JsonPropertyName("listId")]
    public Guid? ListId { get; set; }

    [JsonPropertyName("bag")]
    public object Bag { get; set; } = null!;
}