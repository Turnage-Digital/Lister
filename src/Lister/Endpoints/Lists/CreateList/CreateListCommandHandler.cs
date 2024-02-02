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
        if (request.CreatedBy is null)
        {
            throw new ArgumentNullException(nameof(request), "CreatedBy is null");
        }

        var created = await _listAggregate.CreateAsync(
            request.CreatedBy,
            request.Name,
            request.Statuses,
            request.Columns,
            cancellationToken);

        var retval = _mapper.Map<ListItemDefinitionView>(created);
        return retval;
    }
}

public abstract class CreateListCommandHandler<TReadOnlyList>
    : IRequestHandler<CreateListCommand<TReadOnlyList>, TReadOnlyList>
    where TReadOnlyList : IReadOnlyList
{
    public abstract Task<TReadOnlyList> Handle(
        CreateListCommand<TReadOnlyList> request,
        CancellationToken cancellationToken = default
    );
}