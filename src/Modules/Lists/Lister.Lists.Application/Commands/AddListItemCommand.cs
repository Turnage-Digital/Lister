using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Lister.Core.Application;
using Lister.Lists.Domain.Entities;

namespace Lister.Lists.Application.Commands;

public record AddListItemCommand : RequestBase<Item>
{
    [JsonPropertyName("listId")]
    [Required]
    public string ListId { get; set; } = null!;

    [JsonPropertyName("bag")]
    [Required]
    public object Bag { get; set; } = null!;
}