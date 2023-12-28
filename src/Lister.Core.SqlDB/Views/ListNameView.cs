using Newtonsoft.Json;

namespace Lister.Core.SqlDB.Views;

public record ListNameView : IReadOnlyList
{
    [JsonProperty("name")]
    public string Name { get; set; } = null!;

    [JsonProperty("id")]
    public Guid? Id { get; set; }
}