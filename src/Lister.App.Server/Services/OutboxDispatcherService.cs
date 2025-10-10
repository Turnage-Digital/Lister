using System.Text.Json;
using Lister.Core.Infrastructure.Sql;
using Microsoft.EntityFrameworkCore;

namespace Lister.App.Server.Services;

public class OutboxDispatcherService(
    ILogger<OutboxDispatcherService> logger,
    IServiceScopeFactory scopeFactory,
    ChangeFeed feed
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Outbox dispatcher started");
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<CoreDbContext>();
                await ProcessPendingOnceAsync(db, feed, logger, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Outbox: loop error");
            }

            try
            {
                var delay = TimeSpan.FromSeconds(2);
                logger.LogTrace("Outbox: sleeping {Delay}s", delay.TotalSeconds);
                await Task.Delay(delay, cancellationToken);
            }
            catch
            {
            }
        }

        logger.LogInformation("Outbox dispatcher stopped");
    }

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

        if (batch.Count == 0)
        {
            logger.LogInformation("Outbox: no messages due at {Now}", now);
        }
        else
        {
            logger.LogInformation(
                "Outbox: dispatching {Count} messages {@Messages}",
                batch.Count,
                batch.Select(m => new { m.Id, m.Type, m.Attempts, m.AvailableAfter })
            );
        }

        foreach (var msg in batch)
        {
            try
            {
                logger.LogInformation("Outbox: dispatching message {@Message}",
                    new { msg.Id, msg.Type, Attempt = msg.Attempts + 1, msg.AvailableAfter });
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
                logger.LogInformation("Outbox: dispatched message {@Message}",
                    new { msg.Id, msg.Type, msg.ProcessedOn, msg.Attempts });
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
                logger.LogError(ex, "Outbox: error dispatching message {@Message}",
                    new { msg.Id, msg.Type, msg.Attempts, NextTry = msg.AvailableAfter });
            }
        }

        if (batch.Count > 0)
        {
            await db.SaveChangesAsync(stoppingToken);
            logger.LogInformation("Outbox: saved changes for {@Messages}",
                batch.Select(m => new { m.Id, m.Type, m.ProcessedOn, m.Attempts }));
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
            if (deleted > 0)
            {
                logger.LogInformation("Outbox: cleaned up {Deleted} old messages", deleted);
            }
        }
        catch (Exception ex)
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
                logger.LogWarning(ex, "Outbox: cleaned up {Deleted} old messages using fallback path", toRemove.Count);
            }
        }
    }
}