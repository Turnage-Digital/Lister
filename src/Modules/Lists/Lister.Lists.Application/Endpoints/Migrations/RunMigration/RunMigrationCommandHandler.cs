using System.Text.Json;
using Lister.Lists.Application.Endpoints.Migrations.Shared;
using Lister.Lists.Domain;
using Lister.Lists.Domain.Entities;
using Lister.Lists.Domain.Queries;
using MediatR;

namespace Lister.Lists.Application.Endpoints.Migrations.RunMigration;

public class RunMigrationCommandHandler<TList, TItem, TMigrationJob>(
    IMigrationValidator validator,
    ListsAggregate<TList, TItem, TMigrationJob> listsAggregate,
    IGetListMigrationJob jobGetter
) : IRequestHandler<RunMigrationCommand, RunMigrationResponse>
    where TList : IWritableList
    where TItem : IWritableItem
    where TMigrationJob : IWritableListMigrationJob
{
    public async Task<RunMigrationResponse> Handle(RunMigrationCommand request, CancellationToken cancellationToken)
    {
        if (request.UserId is null)
        {
            throw new ArgumentNullException(nameof(request), "request.UserId cannot be null");
        }

        var dryRun = await validator.ValidateAsync(request.ListId, request.Plan, cancellationToken);

        if (request.Mode == MigrationMode.DryRun || !dryRun.IsSafe)
        {
            return new RunMigrationResponse(dryRun, null, false);
        }

        var planJson = JsonSerializer.Serialize(request.Plan);
        var (jobId, created) = await listsAggregate.EnsureMigrationJobQueuedAsync(
            request.ListId,
            request.UserId,
            planJson,
            cancellationToken);

        var readModel = await jobGetter.GetAsync(request.ListId, jobId, cancellationToken);
        var summary = readModel is null ? null : MigrationJobMapper.ToSummary(readModel);

        return new RunMigrationResponse(dryRun, summary, created);
    }
}