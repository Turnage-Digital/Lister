using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Lister.Lists.Application.Endpoints.ConvertTextToListItem;

public record ConvertTextToListItemRequest
{
    [JsonPropertyName("text")]
    [Required]
    public string Text { get; set; } = string.Empty;
}