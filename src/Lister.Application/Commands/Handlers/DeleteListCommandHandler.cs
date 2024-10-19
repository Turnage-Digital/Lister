using Lister.Domain;
using MediatR;

namespace Lister.Application.Commands.List.Handlers;

public class DeleteListCommandHandler<TList>(ListAggregate<TList> listAggregate)
    : IRequestHandler<DeleteListCommand>
    where TList : IWritableList
{
    public async Task Handle(
        DeleteListCommand request,
        CancellationToken cancellationToken = default)
    {
        var list = await listAggregate.ReadAsync(request.ListId, cancellationToken);
        if (list is null)
            throw new ArgumentNullException(nameof(request), $"List with id {request.ListId} does not exist");

        if (request.UserId is null)
            throw new ArgumentNullException(nameof(request), "UserId is null");

        await listAggregate.DeleteAsync(list, request.UserId, cancellationToken);
    }
}