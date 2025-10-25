using Lister.Lists.Domain;
using Lister.Lists.Domain.Entities;
using Lister.Lists.Domain.Queries;
using Lister.Lists.Domain.Views;
using MediatR;

namespace Lister.Lists.Application.Endpoints.CreateList;

public class CreateListCommandHandler<TList, TItem, TMigrationJob>(
    ListsAggregate<TList, TItem, TMigrationJob> listsAggregate,
    IGetListItemDefinition definitionGetter
) : IRequestHandler<CreateListCommand, ListItemDefinition>
    where TList : IWritableList
    where TItem : IWritableItem
    where TMigrationJob : IWritableListMigrationJob
{
    public async Task<ListItemDefinition> Handle(
        CreateListCommand request,
        CancellationToken cancellationToken
    )
    {
        if (request.UserId is null)
        {
            throw new ArgumentNullException(nameof(request), "request.UserId cannot be null");
        }

        var created = await listsAggregate.CreateListAsync(
            request.UserId,
            request.Name,
            request.Statuses,
            request.Columns,
            request.Transitions,
            cancellationToken);

        var retval = await definitionGetter.GetAsync(created.Id!.Value, cancellationToken)
                     ?? throw new InvalidOperationException("Created list definition not found");
        return retval;
    }
}