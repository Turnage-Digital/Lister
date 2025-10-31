using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Lister.Lists.Domain.ValueObjects;

namespace Lister.Lists.Application.Endpoints.Commands.CreateList;

public record CreateListRequest
{
    [JsonPropertyName("name")]
    [Required]
    public string Name { get; set; } = null!;

    [JsonPropertyName("statuses")]
    [Required]
    public Status[] Statuses { get; set; } = null!;

    [JsonPropertyName("columns")]
    [Required]
    public Column[] Columns { get; set; } = null!;

    [JsonPropertyName("transitions")]
    public StatusTransition[]? Transitions { get; set; }
}