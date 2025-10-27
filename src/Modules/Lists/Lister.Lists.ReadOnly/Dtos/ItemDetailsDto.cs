using System.Text.Json.Serialization;
using Lister.Core.Domain.ValueObjects;
using Lister.Lists.Domain.Enums;

namespace Lister.Lists.ReadOnly.Dtos;

public record ItemDetailsDto
{
    [JsonPropertyName("bag")]
    public object Bag { get; init; } = null!;

    [JsonPropertyName("history")]
    public Entry<ItemHistoryType>[] History { get; init; } = [];

    [JsonPropertyName("id")]
    public int? Id { get; init; }

    [JsonPropertyName("listId")]
    public Guid? ListId { get; init; }
}
