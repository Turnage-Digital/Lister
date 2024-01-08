using System.ComponentModel.DataAnnotations;
using Lister.Core.ValueObjects;
using MediatR;
using Newtonsoft.Json;

namespace Lister.Endpoints.Lists.CreateListItem;

public class CreateListItemCommand : IRequest<Item>
{
    [JsonIgnore]
    public string? CreatedBy { get; set; }

    [JsonProperty("listId")]
    [Required]
    public string ListId { get; set; } = null!;

    [JsonProperty("bag")]
    [Required]
    public object Bag { get; set; } = null!;
}