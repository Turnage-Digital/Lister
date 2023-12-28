using Lister.Core.ValueObjects;
using MediatR;
using Newtonsoft.Json;

namespace Lister.Application.Commands;

public class CreateListItemCommand : IRequest<Item>
{
    [JsonIgnore]
    public string CreatedBy { get; set; } = null!;

    [JsonProperty("listId")]
    public Guid ListId { get; set; }

    [JsonProperty("bag")]
    public object Bag { get; set; } = null!;
}