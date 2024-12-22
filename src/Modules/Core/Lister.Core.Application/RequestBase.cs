using System.Text.Json.Serialization;
using MediatR;

namespace Lister.Core.Application;

public abstract record RequestBase : IRequest
{
    [JsonIgnore] public string? UserId { get; set; }
}

public abstract record RequestBase<T> : IRequest<T>
{
    [JsonIgnore] public string? UserId { get; set; }
}