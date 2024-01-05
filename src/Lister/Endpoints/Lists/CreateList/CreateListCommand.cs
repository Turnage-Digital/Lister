using Lister.Core;
using Lister.Core.ValueObjects;
using MediatR;
using Newtonsoft.Json;

namespace Lister.Endpoints.Lists.CreateList;

public class CreateListCommand<TList> : IRequest<TList>
    where TList : IReadOnlyList
{
    [JsonIgnore]
    public string? CreatedBy { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; } = null!;

    [JsonProperty("statuses")]
    public Status[] Statuses { get; set; } = null!;

    [JsonProperty("columns")]
    public Column[] Columns { get; set; } = null!;
}