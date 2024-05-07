using System.ComponentModel.DataAnnotations;
using Lister.Core.ValueObjects;
using Newtonsoft.Json;

namespace Lister.Application.Commands;

public class ConvertTextToListItemCommand : RequestBase<Item>
{
    [JsonProperty("listId")]
    [Required]
    public string ListId { get; set; } = null!;

    [JsonProperty("text")]
    [Required]
    public string Text { get; set; } = null!;
}