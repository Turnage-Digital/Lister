using AutoMapper;
using Lister.Core;
using Lister.Domain;
using MediatR;

namespace Lister.Application.Commands;

public class CreateListDefCommandHandler<TReadOnlyListDef, TWritableListDef>
    : IRequestHandler<CreateListDefCommand<TReadOnlyListDef>, TReadOnlyListDef>
    where TReadOnlyListDef : IReadOnlyListDef
    where TWritableListDef : IWritableListDef
{
    private readonly ListDefAggregate<TWritableListDef> _listDefAggregate;
    private readonly IMapper _mapper;

    public CreateListDefCommandHandler(
        ListDefAggregate<TWritableListDef> listDefAggregate,
        IMapper mapper
    )
    {
        _listDefAggregate = listDefAggregate;
        _mapper = mapper;
    }

    public async Task<TReadOnlyListDef> Handle(
        CreateListDefCommand<TReadOnlyListDef> request,
        CancellationToken cancellationToken = default
    )
    {
        var created = await _listDefAggregate.CreateAsync(
            request.CreatedBy,
            request.Name,
            request.StatusDefs,
            request.ColumnDefs,
            cancellationToken);

        var retval = _mapper.Map<TReadOnlyListDef>(created);
        return retval;
    }
}