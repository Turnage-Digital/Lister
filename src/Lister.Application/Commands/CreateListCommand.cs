using Lister.Core;
using Lister.Core.ValueObjects;
using MediatR;
using Newtonsoft.Json;

namespace Lister.Application.Commands;

public class CreateListCommand<TList> : IRequest<TList>
    where TList : IReadOnlyList
{
    [JsonIgnore]
    public string CreatedBy { get; set; } = null!;

    [JsonProperty("name")]
    public string Name { get; set; } = null!;

    [JsonProperty("statuses")]
    public Status[] Statuses { get; set; } = null!;

    [JsonProperty("columns")]
    public Column[] Columns { get; set; } = null!;
}