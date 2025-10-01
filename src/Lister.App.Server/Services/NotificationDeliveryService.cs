using Lister.Notifications.Domain;
using Lister.Notifications.Domain.Entities;
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
        logger.LogInformation("NotificationDelivery: service started");

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
                var notifications = pending as IWritableNotification[] ?? pending.ToArray();
                if (notifications.Length is 0)
                {
                    logger.LogInformation("NotificationDelivery: no pending notifications due at {Now}",
                        DateTime.UtcNow);
                }
                else
                {
                    logger.LogInformation(
                        "NotificationDelivery: processing {Count} notifications {@Notifications}",
                        notifications.Length,
                        notifications.Select(n => new { n.Id, n.UserId, n.ListId, n.ItemId })
                    );
                }

                foreach (var notification in notifications)
                {
                    if (notification.Id is null)
                    {
                        logger.LogWarning(
                            "NotificationDelivery: encountered pending notification with null Id {@Notification}",
                            new { notification.UserId, notification.ListId, notification.ItemId }
                        );
                        continue;
                    }

                    // Reload concrete entity so aggregate operations work
                    var dbNotification = await aggregate.GetNotificationByIdAsync(notification.Id.Value,
                        notification.UserId, stoppingToken);
                    if (dbNotification is null)
                    {
                        logger.LogWarning(
                            "NotificationDelivery: notification not found when reloading {@Notification}",
                            new { notification.Id, notification.UserId }
                        );
                        continue;
                    }

                    logger.LogInformation(
                        "NotificationDelivery: loaded {@Notification}",
                        new { dbNotification.Id, dbNotification.UserId, dbNotification.ListId, dbNotification.ItemId }
                    );
                    var channels = new List<NotificationChannel>();
                    if (dbNotification.NotificationRuleId is not null)
                    {
                        var rule = await unitOfWork.RulesStore.GetByIdAsync(dbNotification.NotificationRuleId.Value,
                            stoppingToken);
                        if (rule is not null)
                        {
                            var ruleChannels = await unitOfWork.RulesStore.GetChannelsAsync(rule, stoppingToken);
                            channels.AddRange(ruleChannels);
                            logger.LogInformation(
                                "NotificationDelivery: rule resolved {@Rule} {@Channels}",
                                new { rule.Id, rule.ListId },
                                ruleChannels
                            );
                        }
                    }

                    if (channels.Count == 0)
                    {
                        channels.Add(NotificationChannel.InApp());
                        logger.LogInformation(
                            "NotificationDelivery: defaulting to InApp channel for {@Notification}",
                            new { dbNotification.Id, dbNotification.UserId }
                        );
                    }

                    // Minimal delivery: mark as Delivered for each channel
                    foreach (var channel in channels)
                    {
                        logger.LogTrace(
                            "NotificationDelivery: marking delivered for {@Notification} via {@Channel}",
                            new { dbNotification.Id, dbNotification.UserId },
                            channel
                        );
                        await aggregate.RecordDeliveryAttemptAsync(
                            dbNotification,
                            channel,
                            DeliveryStatus.Delivered,
                            null,
                            stoppingToken
                        );
                    }

                    await aggregate.MarkNotificationAsProcessedAsync(dbNotification, stoppingToken);
                    logger.LogInformation(
                        "NotificationDelivery: processed {@Notification} with {Count} channel(s)",
                        new { dbNotification.Id, dbNotification.UserId },
                        channels.Count
                    );
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "NotificationDelivery: loop failed");
            }

            try
            {
                var delay = TimeSpan.FromSeconds(5);
                logger.LogTrace("NotificationDelivery: sleeping {Delay}s", delay.TotalSeconds);
                await Task.Delay(delay, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                // ignore
            }
        }

        logger.LogInformation("NotificationDelivery: service stopped");
    }
}