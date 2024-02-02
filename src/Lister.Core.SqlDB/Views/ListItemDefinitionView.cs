using Lister.Core.ValueObjects;
using Newtonsoft.Json;

namespace Lister.Core.SqlDB.Views;

public record ListItemDefinitionView : IReadOnlyList
{
    [JsonProperty("name")]
    public string Name { get; set; } = null!;

    [JsonProperty("columns")]
    public Column[] Columns { get; set; } = null!;

    [JsonProperty("statuses")]
    public Status[] Statuses { get; set; } = null!;

    [JsonProperty("id")]
    public Guid? Id { get; set; }
}