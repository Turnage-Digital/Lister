using System.Text.Json;
using Lister.Lists.Application.Endpoints.Migrations.RunMigration;
using Lister.Lists.Domain.Views;

namespace Lister.Lists.Application.Endpoints.Migrations.Shared;

public static class MigrationJobMapper
{
    public static MigrationJobSummary ToSummary(ListMigrationJob job)
    {
        return new MigrationJobSummary(
            job.Id,
            job.ListId,
            job.RequestedByUserId,
            job.Status,
            job.RequestedOn,
            job.StartedOn,
            job.CompletedOn,
            job.FailedOn,
            job.CanceledOn,
            job.LastProgressOn,
            job.ProgressPercent,
            job.TotalItems,
            job.ProcessedItems,
            job.CurrentMessage,
            job.FailureReason,
            job.CancelRequested
        );
    }

    public static MigrationJobDetails ToDetails(ListMigrationJob job, MigrationPlan? plan)
    {
        return new MigrationJobDetails(
            job.Id,
            job.ListId,
            job.RequestedByUserId,
            job.Status,
            job.RequestedOn,
            job.StartedOn,
            job.CompletedOn,
            job.FailedOn,
            job.CanceledOn,
            job.BackupCompletedOn,
            job.LastProgressOn,
            job.ProgressPercent,
            job.TotalItems,
            job.ProcessedItems,
            job.CurrentMessage,
            job.FailureReason,
            job.CancelRequested,
            job.CancelRequestedByUserId,
            job.CancelRequestedOn,
            job.BackupListId,
            job.BackupListName,
            plan
        );
    }

    public static MigrationPlan? DeserializePlan(string? planJson)
    {
        if (string.IsNullOrWhiteSpace(planJson))
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize<MigrationPlan>(planJson);
        }
        catch
        {
            return null;
        }
    }
}