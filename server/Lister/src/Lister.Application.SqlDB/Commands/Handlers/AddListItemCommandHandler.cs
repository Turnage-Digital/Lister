using Lister.Application.Commands;
using Lister.Application.Commands.Handlers;
using Lister.Core.SqlDB.Entities;
using Lister.Core.ValueObjects;
using Lister.Domain;

namespace Lister.Application.SqlDB.Commands.Handlers;

public class AddListItemCommandHandler(ListAggregate<ListEntity, ItemEntity> listAggregate)
    : AddListItemCommandHandlerBase
{
    public override async Task<Item> Handle(AddListItemCommand request,
        CancellationToken cancellationToken = default)
    {
        var list = await listAggregate.ReadAsync(request.ListId!, cancellationToken);
        if (list is null)
            throw new ArgumentNullException(nameof(request), $"List with id {request.ListId} does not exist");

        if (request.UserId is null)
            throw new ArgumentNullException(nameof(request), "UserId is null");

        var retval = await listAggregate.AddListItemAsync(list, request.UserId, request.Bag, cancellationToken);
        return retval;
    }
}