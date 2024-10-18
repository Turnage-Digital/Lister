using AutoMapper;
using Lister.Domain;
using Lister.Domain.Entities;
using Lister.Domain.Views;
using MediatR;

namespace Lister.Application.Commands;

public class CreateListCommandHandler<TList, TItem>(
    ListAggregate<TList, TItem> listAggregate,
    IMapper mapper)
    : IRequestHandler<CreateListCommand, ListItemDefinition>
    where TList : IWritableList
    where TItem : Item
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