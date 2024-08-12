using Lister.Application.Commands;
using Lister.Core.SqlDB.Entities;
using Lister.Domain;
using MediatR;

namespace Lister.Application.SqlDB.Commands;

public class DeleteListCommandHandler(ListAggregate<ListEntity, ItemEntity> listAggregate)
    : DeleteListCommandHandlerBase
{
    public override async Task<Unit> Handle(
        DeleteListCommand request,
        CancellationToken cancellationToken = default)
    {
        var list = await listAggregate.ReadAsync(request.ListId, cancellationToken);
        if (list is null)
            throw new ArgumentNullException(nameof(request), $"List with id {request.ListId} does not exist");

        if (request.UserId is null)
            throw new ArgumentNullException(nameof(request), "UserId is null");

        await listAggregate.DeleteAsync(list, request.UserId, cancellationToken);

        return Unit.Value;
    }
}