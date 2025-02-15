using System.Text.Json.Serialization;

namespace Lister.Lists.Domain.Views;

public record ListName : IReadOnlyList
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("count")]
    public int Count { get; set; }

    [JsonPropertyName("id")]
    public Guid? Id { get; set; }
}