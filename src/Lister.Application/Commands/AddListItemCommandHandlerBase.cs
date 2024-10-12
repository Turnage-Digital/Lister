using Lister.Core.Entities;
using MediatR;

namespace Lister.Application.Commands;

public abstract class AddListItemCommandHandlerBase
    : IRequestHandler<AddListItemCommand, Item>
{
    public abstract Task<Item> Handle(
        AddListItemCommand request,
        CancellationToken cancellationToken = default
    );
}