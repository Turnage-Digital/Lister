using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Lister.Domain.Entities;

namespace Lister.Application.Commands;

public record AddListItemCommand : RequestBase<Item>
{
    [JsonPropertyName("listId")]
    [Required]
    public string? ListId { get; set; }

    [JsonPropertyName("bag")]
    [Required]
    public object Bag { get; set; } = null!;
}