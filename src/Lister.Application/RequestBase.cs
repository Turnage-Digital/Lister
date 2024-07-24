using System.Text.Json.Serialization;
using MediatR;

namespace Lister.Application;

public abstract class RequestBase : IRequest
{
    [JsonIgnore]
    public string? UserId { get; set; }
}

public abstract class RequestBase<T> : IRequest<T>
{
    [JsonIgnore]
    public string? UserId { get; set; }
}