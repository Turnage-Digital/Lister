using Newtonsoft.Json;
using Lister.Core.ValueObjects;

namespace Lister.Core.SqlDB.Views;

public record ListDefView : IReadOnlyListDef
{
    [JsonProperty("name")]
    public string Name { get; set; } = null!;

    [JsonProperty("statusDefs")]
    public StatusDef[] StatusDefs { get; set; } = null!;

    [JsonProperty("columnDefs")]
    public ColumnDef[] ColumnDefs { get; set; } = null!;
    
    [JsonProperty("createdBy")]
    public string CreatedBy { get; set; } = null!;

    [JsonProperty("createdOn")]
    public DateTime CreatedOn { get; set; }

    [JsonProperty("id")]
    public Guid? Id { get; set; }
}