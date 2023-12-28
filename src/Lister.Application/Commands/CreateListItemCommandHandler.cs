using Lister.Core.ValueObjects;
using MediatR;

namespace Lister.Application.Commands;

public class CreateListItemCommandHandler : IRequestHandler<CreateListItemCommand, Item>
{
    public async Task<Item> Handle(CreateListItemCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}