using AutoMapper;
using Lister.Core;
using Lister.Domain;
using MediatR;

namespace Lister.Application.Commands;

public class CreateListCommandHandler<TReadOnlyList, TWritableList>
    : IRequestHandler<CreateListCommand<TReadOnlyList>, TReadOnlyList>
    where TReadOnlyList : IReadOnlyList
    where TWritableList : IWritableList
{
    private readonly ListAggregate<TWritableList> _listAggregate;
    private readonly IMapper _mapper;

    public CreateListCommandHandler(
        ListAggregate<TWritableList> listAggregate,
        IMapper mapper
    )
    {
        _listAggregate = listAggregate;
        _mapper = mapper;
    }

    public async Task<TReadOnlyList> Handle(
        CreateListCommand<TReadOnlyList> request,
        CancellationToken cancellationToken = default
    )
    {
        var created = await _listAggregate.CreateAsync(
            request.CreatedBy,
            request.Name,
            request.Statuses,
            request.Columns,
            cancellationToken);

        var retval = _mapper.Map<TReadOnlyList>(created);
        return retval;
    }
}