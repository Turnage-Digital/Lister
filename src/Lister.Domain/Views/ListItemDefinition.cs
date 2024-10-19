using System.Text.Json.Serialization;
using Lister.Domain.ValueObjects;

namespace Lister.Domain.Views;

public record ListItemDefinition : IReadOnlyList
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("columns")]
    public Column[] Columns { get; set; } = null!;

    [JsonPropertyName("statuses")]
    public Status[] Statuses { get; set; } = null!;

    [JsonPropertyName("id")]
    public Guid? Id { get; set; }
}