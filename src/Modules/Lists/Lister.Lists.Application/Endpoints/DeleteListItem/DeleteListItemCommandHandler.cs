using Lister.Lists.Domain;
using Lister.Lists.Domain.Entities;
using MediatR;

namespace Lister.Lists.Application.Endpoints.DeleteListItem;

public class DeleteListItemCommandHandler<TList, TItem>(ListsAggregate<TList, TItem> listsAggregate)
    : IRequestHandler<DeleteListItemCommand>
    where TList : IWritableList
    where TItem : IWritableItem
{
    public async Task Handle(DeleteListItemCommand request, CancellationToken cancellationToken)
    {
        if (request.UserId is null)
            throw new ArgumentNullException(nameof(request), $"{request.UserId} cannot be null");

        var parsed = Guid.Parse(request.ListId);
        var list = await listsAggregate.GetListByIdAsync(parsed, cancellationToken);
        if (list is null)
            throw new InvalidOperationException($"List with id {request.ListId} does not exist");

        var item = await listsAggregate.GetItemByIdAsync(list, request.ItemId, cancellationToken);
        if (item is null)
            throw new InvalidOperationException($"Item with id {request.ItemId} does not exist");

        await listsAggregate.DeleteItemAsync(item, request.UserId, cancellationToken);
    }
}