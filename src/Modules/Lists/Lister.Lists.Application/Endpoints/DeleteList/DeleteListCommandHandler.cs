using Lister.Lists.Domain;
using MediatR;

namespace Lister.Lists.Application.Endpoints.DeleteList;

public class DeleteListCommandHandler<TList>(ListsAggregate<TList> listsAggregate)
    : IRequestHandler<DeleteListCommand>
    where TList : IWritableList
{
    public async Task Handle(
        DeleteListCommand request,
        CancellationToken cancellationToken = default)
    {
        if (request.UserId is null)
            throw new ArgumentNullException(nameof(request), "UserId is null");

        var parsed = Guid.Parse(request.ListId);
        var list = await listsAggregate.GetByIdAsync(parsed, request.UserId, cancellationToken);
        if (list is null)
            throw new InvalidOperationException($"List with id {request.ListId} does not exist");

        await listsAggregate.DeleteAsync(list, request.UserId, cancellationToken);
    }
}