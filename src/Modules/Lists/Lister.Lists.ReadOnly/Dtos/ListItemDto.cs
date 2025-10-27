using System.Text.Json.Serialization;

namespace Lister.Lists.ReadOnly.Dtos;

public record ListItemDto
{
    [JsonPropertyName("bag")]
    public object Bag { get; init; } = null!;

    [JsonPropertyName("id")]
    public int? Id { get; init; }

    [JsonPropertyName("listId")]
    public Guid? ListId { get; init; }
}
