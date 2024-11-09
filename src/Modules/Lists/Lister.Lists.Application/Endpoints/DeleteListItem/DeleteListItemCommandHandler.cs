using Lister.Lists.Domain;
using Lister.Lists.Domain.Entities;
using MediatR;

namespace Lister.Lists.Application.Endpoints.DeleteListItem;

public class DeleteListItemCommandHandler<TList>(ListsAggregate<TList> listsAggregate)
    : IRequestHandler<DeleteListItemCommand>
    where TList : IWritableList
{
    public async Task Handle(DeleteListItemCommand request, CancellationToken cancellationToken = default)
    {
        if (request.UserId is null)
            throw new ArgumentNullException(nameof(request), $"{request.UserId} cannot be null");

        var parsed = Guid.Parse(request.ListId);
        var list = await listsAggregate.GetByIdAsync(parsed, cancellationToken);
        if (list is null)
            throw new InvalidOperationException($"List with id {request.ListId} does not exist");
        
        var item = await listsAggregate.GetListItemByIdAsync(request.ItemId, cancellationToken);
        if (item is null)
            throw new InvalidOperationException($"Item with id {request.ItemId} does not exist");

        await listsAggregate.DeleteListItemAsync(list, item, request.UserId, cancellationToken);
    }
}