using AutoMapper;
using Lister.Application.Commands;
using Lister.Application.Commands.Handlers;
using Lister.Core.SqlDB.Entities;
using Lister.Core.SqlDB.Views;
using Lister.Domain;

namespace Lister.Application.SqlDB.Commands.Handlers;

public class CreateListCommandHandler(ListAggregate<ListEntity> listAggregate, IMapper mapper)
    : CreateListCommandHandlerBase<ListItemDefinitionView>
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