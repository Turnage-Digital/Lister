using Lister.Core;
using Lister.Core.ValueObjects;
using MediatR;

namespace Lister.Application.Commands.Handlers;

public abstract class ConvertTextToListItemCommandHandlerBase<TList>
    : IRequestHandler<ConvertTextToListItemCommand, Item>
    where TList : IWritableList
{
    public abstract Task<Item> Handle(
        ConvertTextToListItemCommand request,
        CancellationToken cancellationToken
    );

    protected abstract Task<TList> GetListAsync(
        Guid listId,
        string? userId,
        CancellationToken cancellationToken
    );
}