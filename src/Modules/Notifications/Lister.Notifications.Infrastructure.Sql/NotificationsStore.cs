using System.Text.Json;
using Lister.Notifications.Domain;
using Lister.Notifications.Domain.Enums;
using Lister.Notifications.Domain.ValueObjects;
using Lister.Notifications.Infrastructure.Sql.Entities;
using Lister.Notifications.Infrastructure.Sql.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Lister.Notifications.Infrastructure.Sql.Lister.Notifications.Infrastructure.Sql;

public class NotificationsStore : INotificationsStore<NotificationDb>
{
    private readonly NotificationsDbContext _context;

    public NotificationsStore(NotificationsDbContext context)
    {
        _context = context;
    }

    public async Task<NotificationDb> InitAsync(string userId, Guid listId, CancellationToken cancellationToken)
    {
        return new NotificationDb
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ListId = listId,
            CreatedOn = DateTime.UtcNow,
            Priority = (int)NotificationPriority.Normal
        };
    }

    public async Task<NotificationDb?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Notifications
            .Include(x => x.DeliveryAttempts)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<NotificationDb>> GetPendingAsync(int batchSize, CancellationToken cancellationToken)
    {
        return await _context.Notifications
            .Where(x => x.ProcessedOn == null)
            .OrderBy(x => x.Priority)
            .ThenBy(x => x.CreatedOn)
            .Take(batchSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<NotificationDb>> GetByUserAsync(
        string userId, 
        DateTime? since, 
        DeliveryStatus? status, 
        CancellationToken cancellationToken)
    {
        var query = _context.Notifications
            .Include(x => x.DeliveryAttempts)
            .Where(x => x.UserId == userId);
        
        if (since.HasValue)
            query = query.Where(x => x.CreatedOn >= since.Value);
        
        if (status.HasValue)
        {
            switch (status.Value)
            {
                case DeliveryStatus.Pending:
                    query = query.Where(x => x.ProcessedOn == null);
                    break;
                case DeliveryStatus.Delivered:
                    query = query.Where(x => x.DeliveredOn != null);
                    break;
                case DeliveryStatus.Failed:
                    query = query.Where(x => x.ProcessedOn != null && x.DeliveredOn == null);
                    break;
            }
        }
        
        return await query.OrderByDescending(x => x.CreatedOn).ToListAsync(cancellationToken);
    }

    public async Task CreateAsync(NotificationDb notification, CancellationToken cancellationToken)
    {
        _context.Notifications.Add(notification);
        
        var historyEntry = new NotificationHistoryEntryDb
        {
            NotificationId = notification.Id,
            Type = NotificationHistoryType.Created,
            On = DateTime.UtcNow,
            By = notification.UserId
        };
        
        notification.History.Add(historyEntry);
    }

    public async Task SetContentAsync(NotificationDb notification, NotificationContent content, CancellationToken cancellationToken)
    {
        notification.ContentJson = JsonSerializer.Serialize(content);
    }

    public async Task SetPriorityAsync(NotificationDb notification, NotificationPriority priority, CancellationToken cancellationToken)
    {
        notification.Priority = (int)priority;
    }

    public async Task SetItemAsync(NotificationDb notification, int itemId, CancellationToken cancellationToken)
    {
        notification.ItemId = itemId;
    }

    public async Task SetRuleAsync(NotificationDb notification, Guid ruleId, CancellationToken cancellationToken)
    {
        notification.NotificationRuleId = ruleId;
    }

    public async Task MarkAsProcessedAsync(NotificationDb notification, DateTime processedOn, CancellationToken cancellationToken)
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
    }

    public async Task MarkAsDeliveredAsync(NotificationDb notification, DateTime deliveredOn, CancellationToken cancellationToken)
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
    }

    public async Task AddDeliveryAttemptAsync(NotificationDb notification, NotificationDeliveryAttempt attempt, CancellationToken cancellationToken)
    {
        var attemptDb = new NotificationDeliveryAttemptDb
        {
            NotificationId = notification.Id,
            AttemptedOn = attempt.On,
            ChannelJson = JsonSerializer.Serialize(attempt.Channel),
            Status = (int)attempt.Type,
            FailureReason = attempt.FailureReason,
            AttemptNumber = attempt.AttemptNumber,
            NextRetryAfter = attempt.NextRetryAfter
        };
        
        notification.DeliveryAttempts.Add(attemptDb);
    }

    public async Task<int> GetDeliveryAttemptCountAsync(NotificationDb notification, NotificationChannel channel, CancellationToken cancellationToken)
    {
        var channelJson = JsonSerializer.Serialize(channel);
        
        return await _context.DeliveryAttempts
            .Where(x => x.NotificationId == notification.Id && x.ChannelJson == channelJson)
            .CountAsync(cancellationToken);
    }
}