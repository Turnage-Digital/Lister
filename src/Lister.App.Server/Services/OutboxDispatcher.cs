using System.Text.Json;
using Lister.Core.Infrastructure.Sql;

namespace Lister.App.Server.Services;

public class OutboxDispatcher(ILogger<OutboxDispatcher> logger, IServiceScopeFactory scopeFactory, ChangeFeed feed)
    : BackgroundService
{
    internal static async Task ProcessPendingOnceAsync(CoreDbContext db, ChangeFeed feed, ILogger logger, CancellationToken stoppingToken)
    {
        var batch = db.OutboxMessages
            .Where(m => m.ProcessedOn == null)
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

                await feed.PublishAsync(new { type = msg.Type, data = payload, occurredOn = msg.CreatedOn }, stoppingToken);
                msg.ProcessedOn = DateTime.UtcNow;
                msg.Attempts += 1;
            }
            catch (Exception ex)
            {
                msg.Attempts += 1;
                msg.LastError = ex.Message;
                logger.LogError(ex, "Outbox loop error while dispatching message {Id}", msg.Id);
            }
        }

        if (batch.Count > 0)
        {
            await db.SaveChangesAsync(stoppingToken);
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
