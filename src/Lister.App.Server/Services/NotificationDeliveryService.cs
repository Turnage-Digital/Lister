using Lister.Notifications.Domain;
using Lister.Notifications.Domain.Enums;
using Lister.Notifications.Domain.Services;
using Lister.Notifications.Domain.ValueObjects;
using Lister.Notifications.Infrastructure.Sql.Entities;

namespace Lister.App.Server.Services;

public class NotificationDeliveryService(
    ILogger<NotificationDeliveryService> logger,
    IServiceScopeFactory scopeFactory
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Notification delivery service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var pendingGetter = scope.ServiceProvider.GetRequiredService<IGetPendingNotifications>();
                var unitOfWork = scope.ServiceProvider
                    .GetRequiredService<INotificationsUnitOfWork<NotificationRuleDb, NotificationDb>>();
                var aggregate = scope.ServiceProvider
                    .GetRequiredService<NotificationAggregate<NotificationRuleDb, NotificationDb>>();

                var pending = await pendingGetter.GetAsync(50, stoppingToken);

                foreach (var n in pending)
                {
                    if (n.Id is null)
                    {
                        continue;
                    }

                    // Reload concrete entity so aggregate operations work
                    var dbNotification = await aggregate.GetNotificationByIdAsync(n.Id.Value, n.UserId, stoppingToken);
                    if (dbNotification is null)
                    {
                        continue;
                    }

                    var channels = new List<NotificationChannel>();
                    if (dbNotification.NotificationRuleId is not null)
                    {
                        var rule = await unitOfWork.RulesStore.GetByIdAsync(dbNotification.NotificationRuleId.Value,
                            stoppingToken);
                        if (rule is not null)
                        {
                            var ruleChannels = await unitOfWork.RulesStore.GetChannelsAsync(rule, stoppingToken);
                            channels.AddRange(ruleChannels);
                        }
                    }

                    if (channels.Count == 0)
                    {
                        channels.Add(NotificationChannel.InApp());
                    }

                    // Minimal delivery: mark as Delivered for each channel
                    foreach (var channel in channels)
                    {
                        await aggregate.RecordDeliveryAttemptAsync(dbNotification, channel, DeliveryStatus.Delivered,
                            null, stoppingToken);
                    }

                    await aggregate.MarkNotificationAsProcessedAsync(dbNotification, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Notification delivery loop failed");
            }

            try
            {
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
            catch (TaskCanceledException)
            {
                // ignore
            }
        }

        logger.LogInformation("Notification delivery service stopped");
    }
}