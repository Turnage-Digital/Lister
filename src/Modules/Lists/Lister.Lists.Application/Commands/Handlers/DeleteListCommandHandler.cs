using Lister.Lists.Domain;
using MediatR;

namespace Lister.Lists.Application.Commands.Handlers;

public class DeleteListCommandHandler<TList>(ListAggregate<TList> listAggregate)
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
        var list = await listAggregate.GetByIdAsync(parsed, request.UserId, cancellationToken);
        if (list is null)
            throw new InvalidOperationException($"List with id {request.ListId} does not exist");

        await listAggregate.DeleteAsync(list, request.UserId, cancellationToken);
    }
}