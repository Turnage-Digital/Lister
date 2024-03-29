using AutoMapper;
using Lister.Core;
using Lister.Core.SqlDB.Entities;
using Lister.Core.SqlDB.Views;
using Lister.Domain;
using MediatR;

namespace Lister.Endpoints.Lists.CreateList;

public class CreateListCommandHandler : CreateListCommandHandler<ListItemDefinitionView>
{
    private readonly ListAggregate<ListEntity> _listAggregate;
    private readonly IMapper _mapper;

    public CreateListCommandHandler(
        ListAggregate<ListEntity> listAggregate,
        IMapper mapper
    )
    {
        _listAggregate = listAggregate;
        _mapper = mapper;
    }

    public override async Task<ListItemDefinitionView> Handle(
        CreateListCommand<ListItemDefinitionView> request,
        CancellationToken cancellationToken = default
    )
    {
        if (request.UserId is null)
        {
            throw new ArgumentNullException(nameof(request), "UserId is null");
        }

        var created = await _listAggregate.CreateAsync(
            request.UserId,
            request.Name,
            request.Statuses,
            request.Columns,
            cancellationToken);

        var retval = _mapper.Map<ListItemDefinitionView>(created);
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