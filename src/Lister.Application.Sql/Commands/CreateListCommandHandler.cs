using AutoMapper;
using Lister.Application.Commands;
using Lister.Core.Sql;
using Lister.Core.Sql.Entities;
using Lister.Core.Views;
using Lister.Domain;

namespace Lister.Application.Sql.Commands;

public class CreateListCommandHandler(
    ListAggregate<ListDb, ItemDb> listAggregate,
    IMapper mapper)
    : CreateListCommandHandlerBase<ListItemDefinition>
{
    public override async Task<ListItemDefinition> Handle(
        CreateListCommand<ListItemDefinition> request,
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

        var retval = mapper.Map<ListItemDefinition>(created);
        return retval;
    }
}