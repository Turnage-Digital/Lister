using Lister.Core.ValueObjects;
using MediatR;
using Newtonsoft.Json;

namespace Lister.Endpoints.Lists.CreateListItem;

public class CreateListItemCommand : IRequest<Item>
{
    [JsonIgnore]
    public string? CreatedBy { get; set; }

    [JsonProperty("listId")]
    public Guid ListId { get; set; }

    [JsonProperty("bag")]
    public object Bag { get; set; } = null!;
}