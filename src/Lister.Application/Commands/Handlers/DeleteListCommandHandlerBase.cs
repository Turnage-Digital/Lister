using MediatR;

namespace Lister.Application.Commands.Handlers;

public abstract class DeleteListCommandHandlerBase
    : IRequestHandler<DeleteListCommand>
{
    public abstract Task Handle(
        DeleteListCommand request,
        CancellationToken cancellationToken = default
    );
}