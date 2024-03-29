using MediatR;
using Newtonsoft.Json;

namespace Lister;

public class RequestBase<T> : IRequest<T>
{
    [JsonIgnore]
    public string? UserId { get; set; }

    [JsonProperty("requestId")]
    public Guid RequestId { get; set; } = Guid.NewGuid();
}