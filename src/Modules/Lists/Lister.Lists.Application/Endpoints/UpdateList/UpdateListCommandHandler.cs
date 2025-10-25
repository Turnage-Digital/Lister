using Lister.Lists.Domain;
using Lister.Lists.Domain.Entities;
using MediatR;

namespace Lister.Lists.Application.Endpoints.UpdateList;

public class UpdateListCommandHandler<TList, TItem, TMigrationJob>(
    ListsAggregate<TList, TItem, TMigrationJob> listsAggregate
) : IRequestHandler<UpdateListCommand>
    where TList : IWritableList
    where TItem : IWritableItem
    where TMigrationJob : IWritableListMigrationJob
{
    public async Task Handle(UpdateListCommand request, CancellationToken cancellationToken)
    {
        if (request.UserId is null)
        {
            throw new ArgumentNullException(nameof(request), "request.UserId cannot be null");
        }

        var list = await listsAggregate.GetListByIdAsync(request.ListId, cancellationToken)
                   ?? throw new InvalidOperationException($"List {request.ListId} not found");

        await listsAggregate.UpdateListAsync(
            list,
            request.Columns,
            request.Statuses,
            request.Transitions,
            request.UserId,
            cancellationToken);
    }
}