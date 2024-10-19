using AutoMapper;
using Lister.Domain;
using Lister.Domain.Views;
using MediatR;

namespace Lister.Application.Commands.List.Handlers;

public class CreateListCommandHandler<TList>(
    ListAggregate<TList> listAggregate,
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