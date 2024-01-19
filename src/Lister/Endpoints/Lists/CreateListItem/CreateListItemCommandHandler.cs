using Lister.Core.SqlDB.Entities;
using Lister.Core.ValueObjects;
using Lister.Domain;
using MediatR;

namespace Lister.Endpoints.Lists.CreateListItem;

public class CreateListItemCommandHandler : IRequestHandler<CreateListItemCommand, Item>
{
    private readonly ListAggregate<ListEntity> _listAggregate;

    public CreateListItemCommandHandler(ListAggregate<ListEntity> listAggregate)
    {
        _listAggregate = listAggregate;
    }

    public async Task<Item> Handle(CreateListItemCommand request, CancellationToken cancellationToken)
    {
        var list = await _listAggregate.ReadAsync(request.ListId!, cancellationToken);
        if (list is null)
        {
            throw new ArgumentNullException(nameof(request), $"List with id {request.ListId} does not exist");
        }

        var retval = await _listAggregate.CreateItemAsync(list, request.Bag, cancellationToken);
        return retval;
    }
}