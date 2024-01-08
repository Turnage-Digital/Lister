using Lister.Core.SqlDB.Entities;
using Lister.Core.ValueObjects;
using Lister.Domain;
using MediatR;

namespace Lister.Endpoints.Lists.CreateListItem;

public class CreateListItemCommandHandler : IRequestHandler<CreateListItemCommand, Item>
{
    private readonly ListAggregate<ListEntity> _listAggregate;
    
    public CreateListItemCommandHandler(ListAggregate<ListEntity> listAggregate)
    {
        _listAggregate = listAggregate;
    }
    
    public Task<Item> Handle(CreateListItemCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}