using AutoMapper;
using Lister.Core;
using Lister.Core.SqlDB.Entities;
using Lister.Core.SqlDB.Views;
using Lister.Domain;
using MediatR;

namespace Lister.Endpoints.Lists.CreateList;

public class CreateListCommandHandler(
    ListAggregate<ListEntity> listAggregate,
    IMapper mapper)
    : CreateListCommandHandler<ListItemDefinitionView>
{
    public override async Task<ListItemDefinitionView> Handle(
        CreateListCommand<ListItemDefinitionView> request,
        CancellationToken cancellationToken = default
    )
    {
        if (request.UserId is null)
            throw new ArgumentNullException(nameof(request), "UserId is null");

        var created = await listAggregate.CreateAsync(
            request.UserId,
            request.Name,
            request.Statuses,
            request.Columns,
            cancellationToken);

        var retval = mapper.Map<ListItemDefinitionView>(created);
        return retval;
    }
}

public abstract class CreateListCommandHandler<TList>
    : IRequestHandler<CreateListCommand<TList>, TList>
    where TList : IReadOnlyList
{
    public abstract Task<TList> Handle(
        CreateListCommand<TList> request,
        CancellationToken cancellationToken = default
    );
}