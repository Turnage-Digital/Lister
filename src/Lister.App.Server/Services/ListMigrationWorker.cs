using Lister.Core.Domain.IntegrationEvents;
using Lister.Lists.Application.Endpoints.Migrations.RunMigration;
using Lister.Lists.Application.Endpoints.Migrations.Shared;
using Lister.Lists.Domain;
using Lister.Lists.Domain.Queries;
using Lister.Lists.Domain.Views;
using Lister.Lists.Infrastructure.Sql.Entities;
using MediatR;

namespace Lister.App.Server.Services;

public class ListMigrationWorker(
    ILogger<ListMigrationWorker> logger,
    IServiceScopeFactory scopeFactory
) : BackgroundService
{
    private static readonly TimeSpan IdleDelay = TimeSpan.FromSeconds(3);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("ListMigrationWorker started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var aggregate = scope.ServiceProvider
                    .GetRequiredService<ListsAggregate<ListDb, ItemDb, ListMigrationJobDb>>();
                var executor = scope.ServiceProvider
                    .GetRequiredService<MigrationExecutor<ListDb, ItemDb, ListMigrationJobDb>>();
                var jobGetter = scope.ServiceProvider.GetRequiredService<IGetListMigrationJob>();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                var job = await aggregate.TryStartNextMigrationJobAsync(stoppingToken);
                if (job is null)
                {
                    await Task.Delay(IdleDelay, stoppingToken);
                    continue;
                }

                await ProcessJobAsync(
                    job,
                    aggregate,
                    executor,
                    jobGetter,
                    mediator,
                    stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "ListMigrationWorker loop error");
                await Task.Delay(IdleDelay, stoppingToken);
            }
        }

        logger.LogInformation("ListMigrationWorker stopped");
    }

    private async Task ProcessJobAsync(
        ListMigrationJobDb job,
        ListsAggregate<ListDb, ItemDb, ListMigrationJobDb> aggregate,
        MigrationExecutor<ListDb, ItemDb, ListMigrationJobDb> executor,
        IGetListMigrationJob jobGetter,
        IMediator mediator,
        CancellationToken stoppingToken
    )
    {
        var jobId = job.Id;
        var listId = job.ListId;
        var correlationId = Guid.NewGuid();
        var jobView = await jobGetter.GetAsync(listId, jobId, stoppingToken);
        if (jobView is null)
        {
            logger.LogWarning("Migration job {JobId} missing read model", jobId);
            await aggregate.FailMigrationJobAsync(job, "Migration metadata unavailable", DateTime.UtcNow,
                stoppingToken);
            await mediator.Publish(
                new ListMigrationFailedIntegrationEvent(listId, jobId, correlationId, "Migration metadata unavailable"),
                stoppingToken);
            return;
        }

        var plan = MigrationJobMapper.DeserializePlan(jobView.PlanJson)
                   ?? throw new InvalidOperationException("Migration plan payload missing or invalid");

        logger.LogInformation("Processing migration job {JobId} for list {ListId}", jobId, listId);

        try
        {
            var backup = await aggregate.CreateMigrationBackupAsync(listId, jobView.RequestedByUserId, stoppingToken);
            await aggregate.MarkMigrationJobBackupCompletedAsync(job, backup.BackupListId, backup.BackupListName,
                DateTime.UtcNow, stoppingToken);

            if (await aggregate.IsMigrationCancellationRequestedAsync(job, stoppingToken))
            {
                await HandleCancellationAsync(aggregate, mediator, jobView, job, correlationId,
                    "Migration canceled before execution", stoppingToken);
                return;
            }

            await aggregate.MarkMigrationJobRunningAsync(job, "Executing migration", DateTime.UtcNow, stoppingToken);

            await mediator.Publish(
                new ListMigrationStartedIntegrationEvent(listId, jobId, correlationId, jobView.RequestedByUserId),
                stoppingToken);

            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);

            async Task ProgressAsync(MigrationProgressSnapshot snapshot, CancellationToken ct)
            {
                await aggregate.UpdateMigrationJobProgressAsync(
                    job,
                    snapshot.ProcessedItems,
                    snapshot.TotalItems,
                    snapshot.Percent,
                    snapshot.Message,
                    DateTime.UtcNow,
                    ct);

                await mediator.Publish(
                    new ListMigrationProgressIntegrationEvent(
                        listId,
                        jobId,
                        correlationId,
                        snapshot.Message,
                        snapshot.Percent),
                    ct);

                if (!linkedCts.IsCancellationRequested)
                {
                    var canceled = await aggregate.IsMigrationCancellationRequestedAsync(job, ct);
                    if (canceled)
                    {
                        linkedCts.Cancel();
                    }
                }
            }

            MigrationExecutionResult executionResult;
            try
            {
                executionResult = await executor.ExecuteAsync(
                    listId,
                    jobView.RequestedByUserId,
                    plan,
                    ProgressAsync,
                    linkedCts.Token);
            }
            catch (OperationCanceledException) when (linkedCts.IsCancellationRequested &&
                                                     !stoppingToken.IsCancellationRequested)
            {
                await HandleCancellationAsync(aggregate, mediator, jobView, job, correlationId,
                    "Migration canceled", stoppingToken);
                return;
            }

            await aggregate.CompleteMigrationJobAsync(
                job,
                executionResult.TotalProcessedItems,
                executionResult.TotalItems,
                DateTime.UtcNow,
                stoppingToken);

            await mediator.Publish(
                new ListMigrationCompletedIntegrationEvent(
                    listId,
                    jobId,
                    correlationId,
                    jobView.RequestedByUserId,
                    executionResult.TotalProcessedItems),
                stoppingToken);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            await HandleCancellationAsync(aggregate, mediator, jobView, job, correlationId,
                "Service stopping", CancellationToken.None);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Migration job {JobId} failed", jobId);
            await aggregate.FailMigrationJobAsync(job, ex.Message, DateTime.UtcNow, stoppingToken);
            await mediator.Publish(
                new ListMigrationFailedIntegrationEvent(listId, jobId, correlationId, ex.Message),
                stoppingToken);
        }
    }

    private static async Task HandleCancellationAsync(
        ListsAggregate<ListDb, ItemDb, ListMigrationJobDb> aggregate,
        IMediator mediator,
        ListMigrationJob jobView,
        ListMigrationJobDb job,
        Guid correlationId,
        string message,
        CancellationToken cancellationToken
    )
    {
        await aggregate.CancelMigrationJobAsync(job, message, DateTime.UtcNow, cancellationToken);
        var jobId = job.Id;
        var listId = job.ListId;
        await mediator.Publish(
            new ListMigrationFailedIntegrationEvent(listId, jobId, correlationId, message),
            cancellationToken);
    }
}