using Lister.Core.Application;
using Lister.Lists.Application.Endpoints.Migrations.RunMigration;
using Lister.Lists.Application.Endpoints.Migrations.Shared;
using Lister.Lists.Domain.Queries;
using MediatR;

namespace Lister.Lists.Application.Endpoints.Migrations.GetListMigrationJobs;

public record GetListMigrationJobsQuery(Guid ListId)
    : RequestBase<MigrationJobSummary[]>;

public class GetListMigrationJobsQueryHandler(
    IGetListMigrationJobs getter
) : IRequestHandler<GetListMigrationJobsQuery, MigrationJobSummary[]>
{
    public async Task<MigrationJobSummary[]> Handle(
        GetListMigrationJobsQuery request,
        CancellationToken cancellationToken
    )
    {
        if (request.UserId is null)
        {
            throw new ArgumentNullException(nameof(request), "request.UserId cannot be null");
        }

        var jobs = await getter.GetAsync(request.ListId, cancellationToken);
        return jobs.Select(MigrationJobMapper.ToSummary).ToArray();
    }
}