using AutoMapper;
using Lister.Lists.Domain;
using Lister.Lists.Domain.Views;
using MediatR;

namespace Lister.Lists.Application.Commands.Handlers;

public class CreateListCommandHandler<TList>(
    ListsAggregate<TList> listsAggregate,
    IMapper mapper)
    : IRequestHandler<CreateListCommand, ListItemDefinition>
    where TList : IWritableList
{
    public async Task<ListItemDefinition> Handle(
        CreateListCommand request,
        CancellationToken cancellationToken = default
    )
    {
        if (request.UserId is null)
            throw new ArgumentNullException(nameof(request), "UserId is null");

        var created = await listsAggregate.CreateAsync(
            request.UserId,
            request.Name,
            request.Statuses,
            request.Columns,
            cancellationToken);

        var retval = mapper.Map<ListItemDefinition>(created);
        return retval;
    }
}