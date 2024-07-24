using System.Text.Json.Serialization;

namespace Lister.Core.SqlDB.Views;

public record ListNameView : IReadOnlyList
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("id")]
    public Guid? Id { get; set; }
}