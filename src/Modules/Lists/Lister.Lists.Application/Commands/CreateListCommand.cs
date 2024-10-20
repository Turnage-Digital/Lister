using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Lister.Core.Application;
using Lister.Lists.Domain.ValueObjects;
using Lister.Lists.Domain.Views;

namespace Lister.Lists.Application.Commands;

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