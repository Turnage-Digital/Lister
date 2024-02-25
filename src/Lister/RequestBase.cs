using MediatR;

namespace Lister;

public class RequestBase<T> : IRequest<T>
{
    public Guid RequestId { get; } = Guid.NewGuid();
}