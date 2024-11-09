using Lister.Lists.Domain;
using Lister.Lists.Domain.Entities;
using MediatR;

namespace Lister.Lists.Application.Endpoints.CreateListItem;

public class CreateListItemCommandHandler<TList>(ListsAggregate<TList> listsAggregate)
    : IRequestHandler<CreateListItemCommand, Item>
    where TList : IWritableList
{
    public async Task<Item> Handle(CreateListItemCommand request, CancellationToken cancellationToken = default)
    {
        if (request.UserId is null)
            throw new ArgumentNullException(nameof(request), $"{request.UserId} cannot be null");

        var parsed = Guid.Parse(request.ListId);
        var list = await listsAggregate.GetByIdAsync(parsed, cancellationToken);
        if (list is null)
            throw new InvalidOperationException($"List with id {request.ListId} does not exist");

        var retval = await listsAggregate.CreateListItemAsync(list, request.Bag, request.UserId, cancellationToken);
        return retval;
    }
}