using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Lister.Core.ValueObjects;

public record StatusDef
{
    [JsonProperty("name")]
    [Required]
    public string Name { get; set; } = null!;

    [JsonProperty("color")]
    [Required]
    public string Color { get; set; } = null!;
}