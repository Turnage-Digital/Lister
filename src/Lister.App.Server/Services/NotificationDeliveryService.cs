using Lister.Notifications.Domain;
using Lister.Notifications.Domain.Entities;
using Lister.Notifications.Domain.Enums;
using Lister.Notifications.Domain.ValueObjects;
using Lister.Notifications.Infrastructure.Sql.Entities;
using Lister.Notifications.ReadOnly.Dtos;
using Lister.Notifications.ReadOnly.Queries;

namespace Lister.App.Server.Services;

public class NotificationDeliveryService(
    ILogger<NotificationDeliveryService> logger,
    IServiceScopeFactory scopeFactory
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("NotificationDelivery: service started");

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var pendingGetter = scope.ServiceProvider.GetRequiredService<IGetPendingNotifications>();
                var unitOfWork = scope.ServiceProvider
                    .GetRequiredService<INotificationsUnitOfWork<NotificationRuleDb, NotificationDb>>();
                var aggregate = scope.ServiceProvider
                    .GetRequiredService<NotificationAggregate<NotificationRuleDb, NotificationDb>>();

                var pending = await pendingGetter.GetAsync(50, cancellationToken);
                var notifications = pending as PendingNotificationDto[] ?? pending.ToArray();
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
                    // Reload concrete entity so aggregate operations work
                    var dbNotification = await aggregate.GetNotificationByIdAsync(notification.Id,
                        notification.UserId, cancellationToken);
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
                            cancellationToken);
                        if (rule is not null)
                        {
                            var ruleChannels = await unitOfWork.RulesStore.GetChannelsAsync(rule, cancellationToken);
                            channels.AddRange(ruleChannels);
                            logger.LogInformation(
                                "NotificationDelivery: rule resolved {@Rule} {@Channels}",
                                new { rule.Id, rule.ListId },
                                ruleChannels
                            );
                        }
                    }

                    if (channels.Count is 0)
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
                            cancellationToken
                        );
                    }

                    await aggregate.MarkNotificationAsProcessedAsync(dbNotification, cancellationToken);
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
                await Task.Delay(delay, cancellationToken);
            }
            catch (TaskCanceledException)
            {
                // ignore
            }
        }

        logger.LogInformation("NotificationDelivery: service stopped");
    }
}
