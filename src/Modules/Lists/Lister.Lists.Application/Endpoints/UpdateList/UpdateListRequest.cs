using System.Text.Json.Serialization;
using Lister.Lists.Domain.ValueObjects;

namespace Lister.Lists.Application.Endpoints.UpdateList;

public class UpdateListRequest
{
    [JsonPropertyName("columns")]
    public Column[]? Columns { get; set; }

    [JsonPropertyName("statuses")]
    public Status[]? Statuses { get; set; }

    [JsonPropertyName("transitions")]
    public StatusTransition[]? Transitions { get; set; }
}

