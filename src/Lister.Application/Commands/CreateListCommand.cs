using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Lister.Core;
using Lister.Core.ValueObjects;

namespace Lister.Application.Commands;

public class CreateListCommand<TList> : RequestBase<TList>
    where TList : IReadOnlyList
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