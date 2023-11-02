using Lister.Core;
using Lister.Core.ValueObjects;
using MediatR;
using Newtonsoft.Json;

namespace Lister.Application.Commands;

public class CreateListDefCommand<TListDef> : IRequest<TListDef>
    where TListDef : IReadOnlyListDef
{
    [JsonIgnore]
    public string CreatedBy { get; set; } = null!;

    [JsonProperty("name")]
    public string Name { get; set; } = null!;

    [JsonProperty("statusDefs")]
    public StatusDef[] StatusDefs { get; set; } = null!;

    [JsonProperty("columnDefs")]
    public ColumnDef[] ColumnDefs { get; set; } = null!;
}