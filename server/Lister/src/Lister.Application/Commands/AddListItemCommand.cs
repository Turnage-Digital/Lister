using System.ComponentModel.DataAnnotations;
using Lister.Core.ValueObjects;
using Newtonsoft.Json;

namespace Lister.Application.Commands;

public class AddListItemCommand : RequestBase<Item>
{
    [JsonIgnore]
    public string? ListId { get; set; }

    [JsonProperty("bag")]
    [Required]
    public object Bag { get; set; } = null!;
}