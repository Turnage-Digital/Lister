using Lister.Core.ValueObjects;
using MediatR;
using Newtonsoft.Json;

namespace Lister.Application.Commands;

public class UpdateListDefCommand : IRequest
{
    [JsonIgnore]
    public string UpdatedBy { get; set; } = null!;

    [JsonProperty("id")]
    public string Id { get; set; } = null!;

    [JsonProperty("name")]
    public string Name { get; set; } = null!;

    [JsonProperty("statusDefs")]
    public StatusDef[] StatusDefs { get; set; } = null!;

    [JsonProperty("columnDefs")]
    public ColumnDef[] ColumnDefs { get; set; } = null!;
}