using System.Text.Json.Serialization;

namespace Lister.Lists.Domain.Views;

public record ListName : IReadOnlyList
{
    [JsonPropertyName("name")] public string Name { get; set; } = null!;

    [JsonPropertyName("id")] public Guid? Id { get; set; }
}