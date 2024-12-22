using System.Text.Json.Serialization;

namespace Lister.Lists.Domain.Views;

public record PagedList : IReadOnlyList
{
    [JsonPropertyName("count")] public long Count { get; set; }

    [JsonPropertyName("items")] public ListItem[] Items { get; set; } = null!;

    [JsonPropertyName("id")] public Guid? Id { get; set; }
}