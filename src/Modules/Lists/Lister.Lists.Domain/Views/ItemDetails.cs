using System.Text.Json.Serialization;
using Lister.Core.Domain.ValueObjects;
using Lister.Lists.Domain.Enums;

namespace Lister.Lists.Domain.Views;

public record ItemDetails : IReadOnlyItem
{
    [JsonPropertyName("bag")]
    public object Bag { get; set; } = null!;

    [JsonPropertyName("history")]
    public Entry<ItemHistoryType>[] History { get; set; } = null!;

    [JsonPropertyName("id")]
    public int? Id { get; set; }

    [JsonPropertyName("listId")]
    public Guid? ListId { get; set; }
}