using System.ComponentModel.DataAnnotations;
using Lister.Core.ValueObjects;
using Newtonsoft.Json;

namespace Lister.Endpoints.Lists.ConvertTextToListItem;

public class ConvertTextToListItemCommand : RequestBase<Item>
{
    [JsonProperty("text")]
    [Required]
    public string Text { get; set; } = null!;

    [JsonProperty("listId")]
    [Required]
    public string ListId { get; set; } = null!;
}