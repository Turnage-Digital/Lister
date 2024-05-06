using System.ComponentModel.DataAnnotations;
using Lister.Application;
using Lister.Core;
using Lister.Core.ValueObjects;
using Newtonsoft.Json;

namespace Lister.Endpoints.Lists.CreateList;

public class CreateListCommand<TList> : RequestBase<TList>
    where TList : IReadOnlyList
{
    [JsonProperty("name")]
    [Required]
    public string Name { get; set; } = null!;

    [JsonProperty("statuses")]
    [Required]
    public Status[] Statuses { get; set; } = null!;

    [JsonProperty("columns")]
    [Required]
    public Column[] Columns { get; set; } = null!;
}