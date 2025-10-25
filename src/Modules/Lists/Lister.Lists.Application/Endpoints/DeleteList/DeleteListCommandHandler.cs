using Lister.Lists.Domain;
using Lister.Lists.Domain.Entities;
using MediatR;

namespace Lister.Lists.Application.Endpoints.DeleteList;

public class DeleteListCommandHandler<TList, TItem, TMigrationJob>(
    ListsAggregate<TList, TItem, TMigrationJob> listsAggregate
)
    : IRequestHandler<DeleteListCommand>
    where TList : IWritableList
    where TItem : IWritableItem
    where TMigrationJob : IWritableListMigrationJob
{
    public async Task Handle(DeleteListCommand request, CancellationToken cancellationToken)
    {
        if (request.UserId is null)
        {
            throw new ArgumentNullException(nameof(request), "request.UserId cannot be null");
        }

        var list = await listsAggregate.GetListByIdAsync(request.ListId, cancellationToken);
        if (list is null)
        {
            throw new InvalidOperationException($"List with id {request.ListId} does not exist");
        }

        await listsAggregate.DeleteListAsync(list, request.UserId, cancellationToken);
    }
}