using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Lister.Lists.Application.Endpoints.Commands.CreateListItem;

public record CreateListItemRequest
{
    [JsonPropertyName("bag")]
    [Required]
    public object Bag { get; set; } = null!;
}