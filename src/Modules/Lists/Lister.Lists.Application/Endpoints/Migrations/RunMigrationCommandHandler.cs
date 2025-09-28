using Lister.Lists.Domain.Entities;
using MediatR;

namespace Lister.Lists.Application.Endpoints.Migrations;

public class RunMigrationCommandHandler<TList, TItem>(
    IMigrationValidator validator,
    MigrationExecutor<TList, TItem> executor
) : IRequestHandler<RunMigrationCommand, MigrationDryRunResult>
    where TList : IWritableList
    where TItem : IWritableItem
{
    public async Task<MigrationDryRunResult> Handle(RunMigrationCommand request, CancellationToken cancellationToken)
    {
        if (request.UserId is null)
        {
            throw new ArgumentNullException(nameof(request), "request.UserId cannot be null");
        }

        var result = await validator.ValidateAsync(request.ListId, request.Plan, cancellationToken);
        if (request.Mode == MigrationMode.DryRun || !result.IsSafe)
        {
            return result;
        }

        // Execute with SSE progress
        return await executor.ExecuteAsync(request.ListId, request.UserId, request.Plan, cancellationToken);
    }
}