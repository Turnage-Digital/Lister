using Lister.Domain;
using Lister.Domain.Entities;
using MediatR;

namespace Lister.Application.Commands;

public class DeleteListCommandHandler<TList, TItem>(ListAggregate<TList, TItem> listAggregate)
    : IRequestHandler<DeleteListCommand>
    where TList : IWritableList
    where TItem : Item
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