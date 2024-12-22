using Lister.Lists.Domain;
using Lister.Lists.Domain.Entities;
using MediatR;

namespace Lister.Lists.Application.Commands.DeleteList;

public class DeleteListCommandHandler<TList, TItem>(ListsAggregate<TList, TItem> listsAggregate)
    : IRequestHandler<DeleteListCommand>
    where TList : IWritableList
    where TItem : IWritableItem
{
    public async Task Handle(DeleteListCommand request, CancellationToken cancellationToken)
    {
        if (request.UserId is null)
            throw new ArgumentNullException(nameof(request), "UserId is null");

        var parsed = Guid.Parse(request.ListId);
        var list = await listsAggregate.GetListByIdAsync(parsed, cancellationToken);
        if (list is null)
            throw new InvalidOperationException($"List with id {request.ListId} does not exist");

        await listsAggregate.DeleteListAsync(list, request.UserId, cancellationToken);
    }
}