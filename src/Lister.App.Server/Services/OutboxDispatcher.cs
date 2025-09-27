using Lister.Core.Infrastructure.Sql.Outbox;

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
                var db = scope.ServiceProvider.GetRequiredService<OutboxDbContext>();

                var batch = db.Messages
                    .Where(m => m.ProcessedOn == null)
                    .OrderBy(m => m.CreatedOn)
                    .Take(50)
                    .ToList();

                foreach (var msg in batch)
                {
                    try
                    {
                        await feed.PublishAsync(
                            new { type = msg.Type, data = msg.PayloadJson, occurredOn = msg.CreatedOn }, stoppingToken);
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