using System.ComponentModel.DataAnnotations;
using Lister.App;
using Lister.Core.ValueObjects;
using Newtonsoft.Json;

namespace Lister.Endpoints.Lists.CreateListItem;

public class CreateListItemCommand : RequestBase<Item>
{
    [JsonIgnore]
    public string? ListId { get; set; }

    [JsonProperty("bag")]
    [Required]
    public object Bag { get; set; } = null!;
}