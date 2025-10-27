using System.Text.Json.Serialization;

namespace Lister.Lists.ReadOnly.Dtos;

public record PagedListDto
{
    [JsonPropertyName("count")]
    public long Count { get; init; }

    [JsonPropertyName("items")]
    public ListItemDto[] Items { get; init; } = [];

    [JsonPropertyName("id")]
    public Guid? Id { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;
}
