using Lister.Core.Domain.IntegrationEvents;
using Lister.Lists.Domain;
using Lister.Lists.Domain.Enums;
using Lister.Lists.Infrastructure.Sql;
using Lister.Lists.Infrastructure.Sql.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Lister.App.Server.Services;

public class ListMigrationDispatcherService(
    ILogger<ListMigrationDispatcherService> logger,
    IServiceScopeFactory scopeFactory
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("ListMigrationDispatcher: service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            ListMigrationJobDb? job = null;
            try
            {
                using var scope = scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ListsDbContext>();
                var runner = scope.ServiceProvider.GetRequiredService<ListMigrationJobRunner>();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IListsUnitOfWork<ListDb, ItemDb>>();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                job = await GetNextJobAsync(dbContext, stoppingToken);
                if (job is null)
                {
                    logger.LogDebug("ListMigrationDispatcher: no pending jobs");
                }
                else
                {
                    await ProcessJobAsync(job, dbContext, runner, logger, stoppingToken);
                }

                await CleanupExpiredBackupsAsync(dbContext, unitOfWork, mediator, logger, stoppingToken);
            }
            catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
            {
                logger.LogError(ex, "ListMigrationDispatcher: loop error");
            }

            try
            {
                var delay = job is null ? TimeSpan.FromSeconds(5) : TimeSpan.FromSeconds(1);
                await Task.Delay(delay, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                // ignore
            }
        }

        logger.LogInformation("ListMigrationDispatcher: service stopped");
    }

    private static async Task<ListMigrationJobDb?> GetNextJobAsync(
        ListsDbContext dbContext,
        CancellationToken ct
    )
    {
        var now = DateTime.UtcNow;
        return await dbContext.ListMigrationJobs
            .Where(j =>
                (j.Stage == ListMigrationJobStage.Pending || j.Stage == ListMigrationJobStage.Failed) &&
                (j.AvailableAfter == null || j.AvailableAfter <= now))
            .OrderBy(j => j.CreatedOn)
            .FirstOrDefaultAsync(ct);
    }

    private static async Task ProcessJobAsync(
        ListMigrationJobDb job,
        ListsDbContext dbContext,
        ListMigrationJobRunner runner,
        ILogger logger,
        CancellationToken ct
    )
    {
        job.Stage = ListMigrationJobStage.Running;
        job.StartedOn ??= DateTime.UtcNow;
        job.Attempts += 1;
        job.LastError = null;
        job.AvailableAfter = null;
        await dbContext.SaveChangesAsync(ct);

        try
        {
            await runner.RunAsync(job, ct);
            job.Stage = ListMigrationJobStage.Completed;
            job.CompletedOn = DateTime.UtcNow;
            job.LastError = null;
            job.AvailableAfter = null;
            await dbContext.SaveChangesAsync(ct);
            logger.LogInformation("ListMigrationDispatcher: completed job {JobId}", job.Id);
        }
        catch (Exception ex)
        {
            var delay = ComputeRetryDelay(job.Attempts);
            job.Stage = ListMigrationJobStage.Pending;
            job.LastError = ex.Message;
            job.AvailableAfter = DateTime.UtcNow.Add(delay);
            await dbContext.SaveChangesAsync(ct);
            logger.LogWarning(ex,
                "ListMigrationDispatcher: job {JobId} failed, will retry after {Delay}",
                job.Id,
                delay);
        }
    }

    private static async Task CleanupExpiredBackupsAsync(
        ListsDbContext dbContext,
        IListsUnitOfWork<ListDb, ItemDb> unitOfWork,
        IMediator mediator,
        ILogger logger,
        CancellationToken ct
    )
    {
        var now = DateTime.UtcNow;
        var expiredJobs = await dbContext.ListMigrationJobs
            .Where(j =>
                j.Stage == ListMigrationJobStage.Completed &&
                j.BackupListId != null &&
                j.BackupRemovedOn == null &&
                j.BackupExpiresOn != null &&
                j.BackupExpiresOn <= now)
            .OrderBy(j => j.BackupExpiresOn)
            .Take(5)
            .ToListAsync(ct);

        foreach (var job in expiredJobs)
        {
            try
            {
                if (job.BackupListId is null)
                {
                    job.BackupRemovedOn = DateTime.UtcNow;
                    job.Stage = ListMigrationJobStage.Archived;
                    await unitOfWork.SaveChangesAsync(ct);
                    continue;
                }

                var backupList = await unitOfWork.ListsStore.GetByIdAsync(job.BackupListId.Value, ct);
                if (backupList != null && !backupList.IsDeleted)
                {
                    await unitOfWork.ListsStore.DeleteAsync(backupList, job.RequestedBy, ct);
                }

                job.BackupRemovedOn = DateTime.UtcNow;
                job.Stage = ListMigrationJobStage.Archived;
                await unitOfWork.SaveChangesAsync(ct);

                await mediator.Publish(
                    new ListMigrationProgressIntegrationEvent(job.SourceListId, job.CorrelationId,
                        "Backup list removed after retention window", 100),
                    ct);

                logger.LogInformation(
                    "ListMigrationDispatcher: removed backup list {BackupListId} for job {JobId}",
                    job.BackupListId,
                    job.Id);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                    "ListMigrationDispatcher: failed to remove backup list for job {JobId}",
                    job.Id);
            }
        }
    }

    private static TimeSpan ComputeRetryDelay(int attempts)
    {
        var seconds = Math.Min(Math.Pow(2, attempts), 300);
        return TimeSpan.FromSeconds(seconds);
    }
}