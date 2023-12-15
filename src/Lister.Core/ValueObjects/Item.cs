using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Lister.Core.ValueObjects;

public record Item
{
    [JsonProperty("bag")]
    [Required]
    public object Bag { get; set; } = null!;
}