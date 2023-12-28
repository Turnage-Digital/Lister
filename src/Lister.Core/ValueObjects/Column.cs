using Lister.Core.Enums;
using Newtonsoft.Json;

namespace Lister.Core.ValueObjects;

public record Column
{
    [JsonProperty("name")]

    public string Name { get; set; } = null!;

    [JsonProperty("type")]

    public ColumnType Type { get; set; }
}