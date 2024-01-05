using Lister.Core.ValueObjects;
using MediatR;

namespace Lister.Endpoints.Lists.CreateListItem;

public class CreateListItemCommandHandler : IRequestHandler<CreateListItemCommand, Item>
{
    public Task<Item> Handle(CreateListItemCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}