using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Lister.Domain.ValueObjects;
using Lister.Domain.Views;

namespace Lister.Application.Commands.List;

public record CreateListCommand : RequestBase<ListItemDefinition>
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
}