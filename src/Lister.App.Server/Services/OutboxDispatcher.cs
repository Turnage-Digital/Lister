using System.Text.Json;
using Lister.Core.Infrastructure.Sql;
using Microsoft.EntityFrameworkCore;

namespace Lister.App.Server.Services;

public class OutboxDispatcher(ILogger<OutboxDispatcher> logger, IServiceScopeFactory scopeFactory, ChangeFeed feed)
    : BackgroundService
{
    internal static async Task ProcessPendingOnceAsync(
        CoreDbContext db,
        ChangeFeed feed,
        ILogger logger,
        CancellationToken stoppingToken
    )
    {
        var now = DateTime.UtcNow;
        var batch = db.OutboxMessages
            .Where(m => m.ProcessedOn == null && (m.AvailableAfter == null || m.AvailableAfter <= now))
            .OrderBy(m => m.CreatedOn)
            .Take(50)
            .ToList();

        foreach (var msg in batch)
        {
            try
            {
                object? payload;
                try
                {
                    payload = JsonSerializer.Deserialize<JsonElement>(msg.PayloadJson);
                }
                catch
                {
                    payload = msg.PayloadJson; // fallback to string
                }

                await feed.PublishAsync(new { type = msg.Type, data = payload, occurredOn = msg.CreatedOn },
                    stoppingToken);
                msg.ProcessedOn = DateTime.UtcNow;
                msg.Attempts += 1;
            }
            catch (Exception ex)
            {
                msg.Attempts += 1;
                msg.LastError = ex.Message;
                // Exponential backoff with jitter, capped
                var baseDelay = TimeSpan.FromSeconds(5);
                var maxDelay = TimeSpan.FromMinutes(5);
                var pow = Math.Min(msg.Attempts, 10);
                var delay = TimeSpan.FromSeconds(baseDelay.TotalSeconds * Math.Pow(2, pow - 1));
                if (delay > maxDelay)
                {
                    delay = maxDelay;
                }

                // small jitter to avoid thundering herd
                var jitterMs = Random.Shared.Next(0, 5000);
                msg.AvailableAfter = DateTime.UtcNow.Add(delay).AddMilliseconds(jitterMs);
                logger.LogError(ex, "Outbox loop error while dispatching message {Id}", msg.Id);
            }
        }

        if (batch.Count > 0)
        {
            await db.SaveChangesAsync(stoppingToken);
        }

        // Retention cleanup: delete processed messages older than 7 days in small batches
        try
        {
            var cutoff = DateTime.UtcNow.AddDays(-7);
            var deleted = await db.OutboxMessages
                .Where(m => m.ProcessedOn != null && m.ProcessedOn < cutoff)
                .OrderBy(m => m.ProcessedOn)
                .Take(100)
                .ExecuteDeleteAsync(stoppingToken);

            if (deleted == 0)
            {
                // no-op
            }
        }
        catch
        {
            // Fallback for providers that don't support ExecuteDeleteAsync
            var cutoff = DateTime.UtcNow.AddDays(-7);
            var toRemove = db.OutboxMessages
                .Where(m => m.ProcessedOn != null && m.ProcessedOn < cutoff)
                .OrderBy(m => m.ProcessedOn)
                .Take(50)
                .ToList();
            if (toRemove.Count > 0)
            {
                db.OutboxMessages.RemoveRange(toRemove);
                await db.SaveChangesAsync(stoppingToken);
            }
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Outbox dispatcher started");
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<CoreDbContext>();
                await ProcessPendingOnceAsync(db, feed, logger, stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Outbox loop error");
            }

            try
            {
                await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
            }
            catch
            {
            }
        }

        logger.LogInformation("Outbox dispatcher stopped");
    }
}