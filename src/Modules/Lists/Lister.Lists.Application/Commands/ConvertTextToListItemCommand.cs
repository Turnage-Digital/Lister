using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Lister.Core.Application;
using Lister.Lists.Domain.Entities;

namespace Lister.Lists.Application.Commands;

public record ConvertTextToListItemCommand : RequestBase<Item>
{
    [JsonPropertyName("listId")]
    [Required]
    public string ListId { get; set; } = null!;

    [JsonPropertyName("text")]
    [Required]
    public string Text { get; set; } = null!;
}