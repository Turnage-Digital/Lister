using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Lister.Application.Commands;

public record DeleteListCommand : RequestBase
{
    [JsonPropertyName("listId")]
    [Required]
    public string ListId { get; set; } = null!;
}