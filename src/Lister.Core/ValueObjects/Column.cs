using System.ComponentModel.DataAnnotations;
using Lister.Core.Enums;
using Newtonsoft.Json;

namespace Lister.Core.ValueObjects;

public record Column
{
    [JsonProperty("name")]
    [Required]
    public string Name { get; set; } = null!;

    [JsonProperty("type")]
    [Required]
    public ColumnType Type { get; set; }
}