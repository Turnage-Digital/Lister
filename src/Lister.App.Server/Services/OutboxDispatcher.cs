using System.Text.Json;
using Lister.Core.Infrastructure.Sql;

namespace Lister.App.Server.Services;

public class OutboxDispatcher(ILogger<OutboxDispatcher> logger, IServiceScopeFactory scopeFactory, ChangeFeed feed)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Outbox dispatcher started");
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<CoreDbContext>();

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

                        await feed.PublishAsync(
                            new { type = msg.Type, data = payload, occurredOn = msg.CreatedOn }, stoppingToken);
                        msg.ProcessedOn = DateTime.UtcNow;
                        msg.Attempts += 1;
                    }
                    catch (Exception ex)
                    {
                        msg.Attempts += 1;
                        msg.LastError = ex.Message;
                        logger.LogError(ex, "Failed to dispatch outbox message {Id}", msg.Id);
                    }
                }

                if (batch.Count > 0)
                {
                    await db.SaveChangesAsync(stoppingToken);
                }
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