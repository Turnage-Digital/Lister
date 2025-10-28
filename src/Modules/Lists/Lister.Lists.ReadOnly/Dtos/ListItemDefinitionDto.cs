using System.Text.Json.Serialization;

namespace Lister.Lists.ReadOnly.Dtos;

public record ListItemDefinitionDto
{
    [JsonPropertyName("columns")]
    public ColumnDto[] Columns { get; init; } = [];

    [JsonPropertyName("statuses")]
    public StatusDto[] Statuses { get; init; } = [];

    [JsonPropertyName("transitions")]
    public StatusTransitionDto[] Transitions { get; init; } = [];

    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("id")]
    public Guid? Id { get; init; }
}