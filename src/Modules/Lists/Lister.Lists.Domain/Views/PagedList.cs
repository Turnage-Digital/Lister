using System.Text.Json.Serialization;

namespace Lister.Lists.Domain.Views;

public record PagedList : IReadOnlyList
{
    [JsonPropertyName("id")]
    public Guid? Id { get; set; }

    [JsonPropertyName("count")]
    public long Count { get; set; }

    [JsonPropertyName("items")]
    public ListItem[] Items { get; set; } = null!;
}