using Lister.Lists.Domain;
using Lister.Lists.Domain.Entities;
using MediatR;

namespace Lister.Lists.Application.Commands.Handlers;

public class AddListItemCommandHandler<TList>(ListAggregate<TList> listAggregate)
    : IRequestHandler<AddListItemCommand, Item>
    where TList : IWritableList
{
    public async Task<Item> Handle(AddListItemCommand request,
        CancellationToken cancellationToken = default)
    {
        if (request.UserId is null)
            throw new ArgumentNullException(nameof(request), $"{request.UserId} cannot be null");

        var parsed = Guid.Parse(request.ListId);
        var list = await listAggregate.GetByIdAsync(parsed, request.UserId, cancellationToken);
        if (list is null)
            throw new InvalidOperationException($"List with id {request.ListId} does not exist");

        var retval = await listAggregate.AddListItemAsync(list, request.UserId, request.Bag, cancellationToken);
        return retval;
    }
}