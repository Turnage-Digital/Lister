using Lister.Core.ValueObjects;
using MediatR;

namespace Lister.Application.Commands;

public abstract class ConvertTextToListItemCommandHandlerBase
    : IRequestHandler<ConvertTextToListItemCommand, Item>
{
    public abstract Task<Item> Handle(
        ConvertTextToListItemCommand request,
        CancellationToken cancellationToken = default
    );
}