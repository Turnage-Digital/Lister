using Lister.Core.Application;
using Lister.Lists.Domain;
using Lister.Lists.Domain.Entities;
using MediatR;

namespace Lister.Lists.Application.Endpoints.Migrations.CancelMigrationJob;

public record CancelMigrationJobCommand(Guid ListId, Guid JobId)
    : RequestBase<bool>;

public class CancelMigrationJobCommandHandler<TList, TItem, TMigrationJob>(
    ListsAggregate<TList, TItem, TMigrationJob> listsAggregate
) : IRequestHandler<CancelMigrationJobCommand, bool>
    where TList : IWritableList
    where TItem : IWritableItem
    where TMigrationJob : IWritableListMigrationJob
{
    public async Task<bool> Handle(CancelMigrationJobCommand request, CancellationToken cancellationToken)
    {
        if (request.UserId is null)
        {
            throw new ArgumentNullException(nameof(request), "request.UserId cannot be null");
        }

        var succeeded = await listsAggregate.RequestMigrationCancellationAsync(
            request.ListId,
            request.JobId,
            request.UserId,
            DateTime.UtcNow,
            cancellationToken);
        return succeeded;
    }
}