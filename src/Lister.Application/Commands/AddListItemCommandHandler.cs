using Lister.Domain;
using Lister.Domain.Entities;
using MediatR;

namespace Lister.Application.Commands;

public class AddListItemCommandHandler<TList, TItem>(ListAggregate<TList, TItem> listAggregate)
    : IRequestHandler<AddListItemCommand, Item>
    where TList : IWritableList
    where TItem : Item
{
    public async Task<Item> Handle(AddListItemCommand request,
        CancellationToken cancellationToken = default)
    {
        if (request.UserId is null)
            throw new ArgumentNullException(nameof(request), "UserId is null");

        var list = await listAggregate.ReadAsync(request.ListId!, cancellationToken);
        if (list is null)
            throw new ArgumentNullException(nameof(request), $"List with id {request.ListId} does not exist");

        var retval = await listAggregate.AddListItemAsync(list, request.UserId, request.Bag, cancellationToken);
        return retval;
    }
}