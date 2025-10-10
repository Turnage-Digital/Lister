using System.Text.Json;
using Lister.Notifications.Domain;
using Lister.Notifications.Domain.Enums;
using Lister.Notifications.Domain.ValueObjects;
using Lister.Notifications.Infrastructure.Sql.Entities;
using Lister.Notifications.Infrastructure.Sql.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Lister.Notifications.Infrastructure.Sql;

public class NotificationsStore(NotificationsDbContext dbContext)
    : INotificationsStore<NotificationDb>
{
    public Task<NotificationDb> InitAsync(string userId, Guid listId, CancellationToken cancellationToken)
    {
        var retval = new NotificationDb
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ListId = listId,
            CreatedOn = DateTime.UtcNow,
            Priority = (int)NotificationPriority.Normal
        };
        return Task.FromResult(retval);
    }

    public async Task<NotificationDb?> GetByIdAsync(Guid id, string userId, CancellationToken cancellationToken)
    {
        var retval = await dbContext.Notifications
            .Include(x => x.DeliveryAttempts)
            .Include(x => x.History)
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId, cancellationToken);
        return retval;
    }

    public Task CreateAsync(NotificationDb notification, CancellationToken cancellationToken)
    {
        dbContext.Notifications.Add(notification);

        var historyEntry = new NotificationHistoryEntryDb
        {
            NotificationId = notification.Id,
            Type = NotificationHistoryType.Created,
            On = DateTime.UtcNow,
            By = notification.UserId
        };

        notification.History.Add(historyEntry);
        return Task.CompletedTask;
    }

    public Task SetContentAsync(
        NotificationDb notification,
        NotificationContent notificationContent,
        CancellationToken cancellationToken
    )
    {
        notification.ContentJson = JsonSerializer.Serialize(notificationContent);
        return Task.CompletedTask;
    }

    public Task SetPriorityAsync(
        NotificationDb notification,
        NotificationPriority notificationPriority,
        CancellationToken cancellationToken
    )
    {
        notification.Priority = (int)notificationPriority;
        return Task.CompletedTask;
    }

    public Task SetItemAsync(NotificationDb notification, int itemId, CancellationToken cancellationToken)
    {
        notification.ItemId = itemId;
        return Task.CompletedTask;
    }

    public Task SetRuleAsync(NotificationDb notification, Guid ruleId, CancellationToken cancellationToken)
    {
        notification.NotificationRuleId = ruleId;
        return Task.CompletedTask;
    }

    public Task MarkAsProcessedAsync(
        NotificationDb notification,
        DateTime processedOn,
        CancellationToken cancellationToken
    )
    {
        notification.ProcessedOn = processedOn;

        var historyEntry = new NotificationHistoryEntryDb
        {
            NotificationId = notification.Id,
            Type = NotificationHistoryType.Processed,
            On = processedOn,
            By = "System"
        };

        notification.History.Add(historyEntry);
        return Task.CompletedTask;
    }

    public Task MarkAsDeliveredAsync(
        NotificationDb notification,
        DateTime deliveredOn,
        CancellationToken cancellationToken
    )
    {
        notification.DeliveredOn = deliveredOn;

        var historyEntry = new NotificationHistoryEntryDb
        {
            NotificationId = notification.Id,
            Type = NotificationHistoryType.Delivered,
            On = deliveredOn,
            By = "System"
        };

        notification.History.Add(historyEntry);
        return Task.CompletedTask;
    }

    public Task AddDeliveryAttemptAsync(
        NotificationDb notification,
        NotificationDeliveryAttempt notificationDeliveryAttempt,
        CancellationToken cancellationToken
    )
    {
        var entity = new NotificationDeliveryAttemptDb
        {
            NotificationId = notification.Id,
            AttemptedOn = notificationDeliveryAttempt.On,
            ChannelJson = JsonSerializer.Serialize(notificationDeliveryAttempt.Channel),
            Status = (int)notificationDeliveryAttempt.Type,
            FailureReason = notificationDeliveryAttempt.FailureReason,
            AttemptNumber = notificationDeliveryAttempt.AttemptNumber,
            NextRetryAfter = notificationDeliveryAttempt.NextRetryAfter
        };

        notification.DeliveryAttempts.Add(entity);
        return Task.CompletedTask;
    }

    public async Task<int> GetDeliveryAttemptCountAsync(
        NotificationDb notification,
        NotificationChannel notificationChannel,
        CancellationToken cancellationToken
    )
    {
        var channelJson = JsonSerializer.Serialize(notificationChannel);
        var retval = await dbContext.DeliveryAttempts
            .Where(x => x.NotificationId == notification.Id && x.ChannelJson == channelJson)
            .CountAsync(cancellationToken);
        return retval;
    }

    // public async Task<int> GetUnreadCountAsync(
    //     string userId,
    //     Guid? listId = null,
    //     CancellationToken cancellationToken = default
    // )
    // {
    //     var query = dbContext.Notifications
    //         .Where(x => x.UserId == userId && x.ReadOn == null);
    //
    //     if (listId.HasValue)
    //     {
    //         query = query.Where(x => x.ListId == listId.Value);
    //     }
    //
    //     return await query.CountAsync(cancellationToken);
    // }

    public Task MarkAsReadAsync(
        NotificationDb notification,
        DateTime readOn,
        CancellationToken cancellationToken = default
    )
    {
        notification.ReadOn = readOn;

        var historyEntry = new NotificationHistoryEntryDb
        {
            NotificationId = notification.Id,
            Type = NotificationHistoryType.Read,
            On = readOn,
            By = notification.UserId
        };

        notification.History.Add(historyEntry);
        return Task.CompletedTask;
    }

    public async Task MarkAllAsReadAsync(
        string userId,
        DateTime readOn,
        DateTime? before = null,
        CancellationToken cancellationToken = default
    )
    {
        var query = dbContext.Notifications
            .Where(x => x.UserId == userId && x.ReadOn == null);

        if (before.HasValue)
        {
            query = query.Where(x => x.CreatedOn <= before.Value);
        }

        var notifications = await query.ToListAsync(cancellationToken);

        foreach (var notification in notifications)
        {
            notification.ReadOn = readOn;

            var historyEntry = new NotificationHistoryEntryDb
            {
                NotificationId = notification.Id,
                Type = NotificationHistoryType.Read,
                On = readOn,
                By = userId
            };

            notification.History.Add(historyEntry);
        }
    }

    public async Task<IEnumerable<NotificationDb>> GetUserNotificationsAsync(
        string userId,
        DateTime? since = null,
        int pageSize = 20,
        int page = 0,
        CancellationToken cancellationToken = default
    )
    {
        var query = dbContext.Notifications
            .Where(x => x.UserId == userId);

        if (since.HasValue)
        {
            query = query.Where(x => x.CreatedOn >= since.Value);
        }

        return await query
            .OrderByDescending(x => x.CreatedOn)
            .Skip(page * pageSize)
            .Take(pageSize)
            .Include(x => x.DeliveryAttempts)
            .ToListAsync(cancellationToken);
    }
}