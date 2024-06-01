using Lister.Application.Commands;
using Lister.Application.Commands.Handlers;
using Lister.Core.SqlDB.Entities;
using Lister.Domain;
using MediatR;

namespace Lister.Application.SqlDB.Commands.Handlers;

public class DeleteListCommandHandler(ListAggregate<ListEntity> listAggregate)
    : DeleteListCommandHandlerBase
{
    public override Task<Unit> Handle(
        DeleteListCommand request,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}