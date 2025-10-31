using Lister.Lists.Application.Mappings;
using Lister.Lists.Domain;
using Lister.Lists.Domain.Entities;
using Lister.Lists.ReadOnly.Dtos;
using MediatR;

namespace Lister.Lists.Application.Commands.CreateListItem;

public class CreateListItemCommandHandler<TList, TItem>(ListsAggregate<TList, TItem> listsAggregate)
    : IRequestHandler<CreateListItemCommand, ListItemDto>
    where TList : IWritableList
    where TItem : IWritableItem
{
    public async Task<ListItemDto> Handle(CreateListItemCommand request, CancellationToken cancellationToken)
    {
        if (request.UserId is null)
        {
            throw new ArgumentNullException(nameof(request), "request.UserId cannot be null");
        }

        var list = await listsAggregate.GetListByIdAsync(request.ListId, cancellationToken);
        if (list is null)
        {
            throw new InvalidOperationException($"List with id {request.ListId} does not exist");
        }

        var entity = await listsAggregate.CreateItemAsync(list, request.Bag, request.UserId, cancellationToken);
        var bag = await listsAggregate.GetItemBagAsync(entity, cancellationToken);
        return ListItemWriteContextMap.ToDto(entity, bag);
    }
}