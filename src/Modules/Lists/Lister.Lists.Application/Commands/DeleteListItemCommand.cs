using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Lister.Core.Application;

namespace Lister.Lists.Application.Commands;

public record DeleteListItemCommand : RequestBase
{
    [JsonPropertyName("listId")]
    [Required]
    public string ListId { get; set; } = null!;

    [JsonPropertyName("itemId")]
    [Required]
    public int ItemId { get; set; }
}