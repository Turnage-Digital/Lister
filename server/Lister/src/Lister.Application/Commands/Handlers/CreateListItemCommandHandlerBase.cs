using Lister.Core.ValueObjects;
using MediatR;

namespace Lister.Application.Commands.Handlers;

public abstract class CreateListItemCommandHandlerBase
    : IRequestHandler<CreateListItemCommand, Item>
{
    public abstract Task<Item> Handle(
        CreateListItemCommand request,
        CancellationToken cancellationToken = default
    );
}