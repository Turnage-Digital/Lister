using Lister.Core;
using Lister.Core.Entities;
using MediatR;

namespace Lister.Application.Commands;

public abstract class CreateListCommandHandlerBase<TList>
    : IRequestHandler<CreateListCommand<TList>, TList>
    where TList : IReadOnlyList
{
    public abstract Task<TList> Handle(
        CreateListCommand<TList> request,
        CancellationToken cancellationToken = default
    );
}