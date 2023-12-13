using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Lister.Core.ValueObjects;

public record ListItem
{
    [JsonProperty("bag")]
    [Required]
    public object Bag { get; set; } = null!;
}