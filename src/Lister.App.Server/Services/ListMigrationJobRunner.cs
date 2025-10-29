using System.Text.Json;
using Lister.Core.Domain.IntegrationEvents;
using Lister.Lists.Application.Endpoints.Migrations;
using Lister.Lists.Domain;
using Lister.Lists.Domain.ValueObjects;
using Lister.Lists.Infrastructure.Sql;
using Lister.Lists.Infrastructure.Sql.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Lister.App.Server.Services;

public class ListMigrationJobRunner(
    IListsUnitOfWork<ListDb, ItemDb> unitOfWork,
    ListsDbContext dbContext,
    IMediator mediator,
    ILogger<ListMigrationJobRunner> logger
)
{
    private const int BatchSize = 250;
    private static readonly JsonSerializerOptions PlanSerializerOptions = new(JsonSerializerDefaults.Web);
    private static readonly TimeSpan DefaultBackupRetention = TimeSpan.FromDays(7);

    public async Task RunAsync(ListMigrationJobDb job, CancellationToken cancellationToken)
    {
        MigrationPlan plan;
        try
        {
            plan = JsonSerializer.Deserialize<MigrationPlan>(job.PlanJson, PlanSerializerOptions) ??
                   throw new InvalidOperationException("Migration plan payload could not be deserialized.");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Invalid migration plan payload.", ex);
        }

        await mediator.Publish(
            new ListMigrationStartedIntegrationEvent(job.SourceListId, job.CorrelationId, job.RequestedBy),
            cancellationToken);

        try
        {
            await ExecuteAsync(job, plan, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "ListMigrationJobRunner: job {JobId} failed for list {ListId}", job.Id,
                job.SourceListId);
            await mediator.Publish(
                new ListMigrationFailedIntegrationEvent(job.SourceListId, job.CorrelationId, ex.Message),
                cancellationToken);
            throw;
        }
    }

    private async Task ExecuteAsync(ListMigrationJobDb job, MigrationPlan plan, CancellationToken ct)
    {
        var list = await unitOfWork.ListsStore.GetByIdAsync(job.SourceListId, ct) ??
                   throw new InvalidOperationException($"Source list {job.SourceListId} not found.");
        if (!list.Id.HasValue)
        {
            throw new InvalidOperationException($"Source list {job.SourceListId} has not been persisted.");
        }

        var originalName = list.Name;

        var columns = await unitOfWork.ListsStore.GetColumnsAsync(list, ct);
        var statuses = await unitOfWork.ListsStore.GetStatusesAsync(list, ct);
        var transitions = await unitOfWork.ListsStore.GetStatusTransitionsAsync(list, ct);

        var context = MigrationPlanApplier.Prepare(plan, columns, statuses, transitions);

        var backupName = await BuildUniqueBackupNameAsync(originalName, ct);
        await unitOfWork.ListsStore.RenameAsync(list, backupName, job.RequestedBy, ct);
        await unitOfWork.SaveChangesAsync(ct);
        job.BackupListId = list.Id;
        job.BackupExpiresOn ??= DateTime.UtcNow.Add(DefaultBackupRetention);

        await mediator.Publish(
            new ListMigrationProgressIntegrationEvent(job.SourceListId, job.CorrelationId,
                $"Renamed source list to '{backupName}'", 10),
            ct);

        var newList = await unitOfWork.ListsStore.InitAsync(originalName, job.RequestedBy, ct);
        await unitOfWork.ListsStore.SetColumnsAsync(newList, context.Columns, job.RequestedBy, ct);
        await unitOfWork.ListsStore.SetStatusesAsync(newList, context.Statuses, job.RequestedBy, ct);
        await unitOfWork.ListsStore.SetStatusTransitionsAsync(newList, context.StatusTransitions, job.RequestedBy, ct);
        await unitOfWork.ListsStore.CreateAsync(newList, ct);
        await unitOfWork.SaveChangesAsync(ct);

        if (!newList.Id.HasValue)
        {
            throw new InvalidOperationException("New list creation failed to generate an identifier.");
        }

        job.NewListId = newList.Id;

        await mediator.Publish(
            new ListMigrationProgressIntegrationEvent(job.SourceListId, job.CorrelationId,
                $"Created destination list '{originalName}'", 20),
            ct);

        var totalItems = await dbContext.Items
            .AsNoTracking()
            .Where(i => i.ListId == job.SourceListId && !i.IsDeleted)
            .CountAsync(ct);

        var processed = await CopyItemsAsync(job, newList, context, totalItems, ct);

        await mediator.Publish(
            new ListMigrationCompletedIntegrationEvent(job.SourceListId, job.CorrelationId, job.RequestedBy,
                processed),
            ct);
    }

    private async Task<int> CopyItemsAsync(
        ListMigrationJobDb job,
        ListDb newList,
        MigrationPlanContext context,
        int totalItems,
        CancellationToken ct
    )
    {
        if (!newList.Id.HasValue)
        {
            throw new InvalidOperationException("Destination list identifier is missing.");
        }

        var listId = newList.Id.Value;
        var processed = 0;
        var lastReportedPercent = 20;
        var lastItemId = 0;

        while (true)
        {
            var batch = await dbContext.Items
                .AsNoTracking()
                .Where(i => i.ListId == job.SourceListId && !i.IsDeleted && i.Id.HasValue && i.Id > lastItemId)
                .OrderBy(i => i.Id)
                .Take(BatchSize)
                .ToListAsync(ct);

            if (batch.Count == 0)
            {
                break;
            }

            foreach (var sourceItem in batch)
            {
                var transformedBag = MigrationPlanApplier.ApplyToItem(context, sourceItem.Bag);
                var newItem = await unitOfWork.ItemsStore.InitAsync(listId, job.RequestedBy, ct);
                await unitOfWork.ItemsStore.SetBagAsync(newItem, transformedBag, job.RequestedBy, ct);
                await unitOfWork.ItemsStore.CreateAsync(newItem, ct);
            }

            await unitOfWork.SaveChangesAsync(ct);

            processed += batch.Count;
            lastItemId = batch[^1].Id!.Value;

            var percent = ComputeProgressPercentage(processed, totalItems);
            if (percent > lastReportedPercent)
            {
                lastReportedPercent = percent;
                await mediator.Publish(
                    new ListMigrationProgressIntegrationEvent(job.SourceListId, job.CorrelationId,
                        $"Copied {processed}/{totalItems} items", percent),
                    ct);
            }
        }

        if (totalItems == 0)
        {
            await mediator.Publish(
                new ListMigrationProgressIntegrationEvent(job.SourceListId, job.CorrelationId, "No items to copy", 95),
                ct);
        }

        return processed;
    }

    private static int ComputeProgressPercentage(int processed, int total)
    {
        if (total == 0)
        {
            return 95;
        }

        var percent = 20 + (int)Math.Round(70.0 * processed / total);
        if (percent > 95)
        {
            percent = 95;
        }

        return percent;
    }

    private async Task<string> BuildUniqueBackupNameAsync(string originalName, CancellationToken ct)
    {
        const int maxLength = 50;
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        for (var attempt = 0; attempt < 10; attempt++)
        {
            var suffix = attempt == 0 ? $" (backup {timestamp})" : $" (backup {timestamp}-{attempt})";
            var maxBaseLength = Math.Max(1, maxLength - suffix.Length);
            var baseName = originalName.Length > maxBaseLength
                ? originalName[..maxBaseLength]
                : originalName;
            var candidate = $"{baseName}{suffix}";
            var exists = await dbContext.Lists.AnyAsync(l => l.Name == candidate, ct);
            if (!exists)
            {
                return candidate;
            }
        }

        for (;;)
        {
            var candidate = $"{Guid.NewGuid():N}".Substring(0, 8);
            var backupName = $"{candidate} (backup {timestamp})";
            if (backupName.Length > maxLength)
            {
                backupName = backupName[..maxLength];
            }

            var exists = await dbContext.Lists.AnyAsync(l => l.Name == backupName, ct);
            if (!exists)
            {
                return backupName;
            }
        }
    }
}