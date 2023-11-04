using Lister.Core;
using Lister.Domain;
using MediatR;

namespace Lister.Application.Commands;

public class UpdateListDefCommandHandler<TWritableListDef>
    : IRequestHandler<UpdateListDefCommand>
    where TWritableListDef : IWritableListDef
{
    private readonly ListDefAggregate<TWritableListDef> _listDefAggregate;

    public UpdateListDefCommandHandler(ListDefAggregate<TWritableListDef> listDefAggregate)
    {
        _listDefAggregate = listDefAggregate;
    }

    public async Task Handle(UpdateListDefCommand request, CancellationToken cancellationToken = default)
    {
        await _listDefAggregate.UpdateAsync(
            request.UpdatedBy,
            request.Id,
            request.Name,
            request.StatusDefs,
            request.ColumnDefs,
            cancellationToken);
    }
}