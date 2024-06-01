using Lister.Core.ValueObjects;
using MediatR;

namespace Lister.Application.Commands.Handlers;

public abstract class ConvertTextToListItemCommandHandlerBase
    : IRequestHandler<ConvertTextToListItemCommand, Item>
{
    public abstract Task<Item> Handle(
        ConvertTextToListItemCommand request,
        CancellationToken cancellationToken = default
    );
}