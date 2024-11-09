using System.Text.Json.Serialization;
using Lister.Core.Domain.ValueObjects;
using Lister.Lists.Domain.Enums;

namespace Lister.Lists.Domain.Entities;

public class Item
{
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    [JsonPropertyName("listId")]
    public Guid? ListId { get; set; }

    [JsonPropertyName("bag")]
    public object Bag { get; set; } = null!;
}