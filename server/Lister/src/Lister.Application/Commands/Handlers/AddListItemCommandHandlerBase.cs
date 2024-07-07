using Lister.Core.ValueObjects;
using MediatR;

namespace Lister.Application.Commands.Handlers;

public abstract class AddListItemCommandHandlerBase
    : IRequestHandler<AddListItemCommand, Item>
{
    public abstract Task<Item> Handle(
        AddListItemCommand request,
        CancellationToken cancellationToken = default
    );
}