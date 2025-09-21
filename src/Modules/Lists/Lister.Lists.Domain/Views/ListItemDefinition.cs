using System.Text.Json.Serialization;
using Lister.Lists.Domain.ValueObjects;

namespace Lister.Lists.Domain.Views;

public record ListItemDefinition : IReadOnlyList
{
    [JsonPropertyName("columns")]
    public Column[] Columns { get; set; } = null!;

    [JsonPropertyName("statuses")]
    public Status[] Statuses { get; set; } = null!;

    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("id")]
    public Guid? Id { get; set; }
}