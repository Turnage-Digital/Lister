using Lister.Core.Application;
using Lister.Lists.Application.Endpoints.Migrations.RunMigration;
using Lister.Lists.Application.Endpoints.Migrations.Shared;
using Lister.Lists.Domain.Queries;
using MediatR;

namespace Lister.Lists.Application.Endpoints.Migrations.GetListMigrationJob;

public record GetListMigrationJobQuery(Guid ListId, Guid JobId)
    : RequestBase<MigrationJobDetails?>;

public class GetListMigrationJobQueryHandler(
    IGetListMigrationJob getter
) : IRequestHandler<GetListMigrationJobQuery, MigrationJobDetails?>
{
    public async Task<MigrationJobDetails?> Handle(
        GetListMigrationJobQuery request,
        CancellationToken cancellationToken
    )
    {
        if (request.UserId is null)
        {
            throw new ArgumentNullException(nameof(request), "request.UserId cannot be null");
        }

        var job = await getter.GetAsync(request.ListId, request.JobId, cancellationToken);
        if (job is null)
        {
            return null;
        }

        var plan = MigrationJobMapper.DeserializePlan(job.PlanJson);
        return MigrationJobMapper.ToDetails(job, plan);
    }
}