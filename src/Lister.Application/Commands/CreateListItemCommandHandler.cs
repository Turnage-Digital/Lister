using MediatR;

namespace Lister.Application.Commands;

public class CreateListItemCommandHandler
    : IRequestHandler<CreateListItemCommand>
{
    public async Task Handle(
        CreateListItemCommand request, CancellationToken cancellationToken
    )
    {
        throw new NotImplementedException();
    }
}