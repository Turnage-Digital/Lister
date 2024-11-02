using Lister.Lists.Domain;
using MediatR;

namespace Lister.Lists.Application.Commands.Handlers;

public class DeleteListItemCommandHandler<TList>(ListsAggregate<TList> listsAggregate)
    : IRequestHandler<DeleteListItemCommand>
    where TList : IWritableList
{
    public async Task Handle(DeleteListItemCommand request,
        CancellationToken cancellationToken = default)
    {
        if (request.UserId is null)
            throw new ArgumentNullException(nameof(request), $"{request.UserId} cannot be null");

        var parsed = Guid.Parse(request.ListId);
        var list = await listsAggregate.GetByIdAsync(parsed, request.UserId, cancellationToken);
        if (list is null)
            throw new InvalidOperationException($"List with id {request.ListId} does not exist");

        await listsAggregate.DeleteListItemAsync(list, request.UserId, request.ItemId, cancellationToken);
    }
}