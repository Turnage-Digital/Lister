using MediatR;

namespace Lister.Application.Commands.Handlers;

public abstract class DeleteListCommandHandlerBase
    : IRequestHandler<DeleteListCommand, Unit>
{
    public abstract Task<Unit> Handle(
        DeleteListCommand request,
        CancellationToken cancellationToken = default
    );
}