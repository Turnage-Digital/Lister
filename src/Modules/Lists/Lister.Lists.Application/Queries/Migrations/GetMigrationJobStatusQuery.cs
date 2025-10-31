using Lister.Core.Application;
using Lister.Lists.ReadOnly.Queries;
using MediatR;

namespace Lister.Lists.Application.Queries.Migrations;

public record GetMigrationJobStatusQuery(Guid ListId, Guid CorrelationId)
    : RequestBase<MigrationProgress?>;

public class GetMigrationJobStatusQueryHandler(IGetMigrationJobStatus getter)
    : IRequestHandler<GetMigrationJobStatusQuery, MigrationProgress?>
{
    public async Task<MigrationProgress?> Handle(
        GetMigrationJobStatusQuery request,
        CancellationToken cancellationToken
    )
    {
        var dto = await getter.GetAsync(request.ListId, request.CorrelationId, cancellationToken);
        if (dto is null)
        {
            return null;
        }

        return new MigrationProgress(
            dto.JobId,
            dto.SourceListId,
            dto.CorrelationId,
            dto.Stage,
            dto.RequestedBy,
            dto.CreatedOn,
            dto.StartedOn,
            dto.CompletedOn,
            dto.BackupListId,
            dto.NewListId,
            dto.BackupExpiresOn,
            dto.BackupRemovedOn,
            dto.Attempts,
            dto.LastError
        );
    }
}