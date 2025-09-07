using System.Text.Json;
using Lister.Notifications.Domain;
using Lister.Notifications.Domain.Enums;
using Lister.Notifications.Domain.ValueObjects;
using Lister.Notifications.Infrastructure.Sql.Entities;
using Lister.Notifications.Infrastructure.Sql.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Lister.Notifications.Infrastructure.Sql;

public class NotificationRulesStore(NotificationsDbContext context) 
    : INotificationRulesStore<NotificationRuleDb>
{
    public Task<NotificationRuleDb> InitAsync(string userId, Guid listId, CancellationToken cancellationToken)
    {
        return Task.FromResult(new NotificationRuleDb
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ListId = listId,
            IsActive = true,
            IsDeleted = false,
            CreatedOn = DateTime.UtcNow,
            CreatedBy = userId
        });
    }

    public async Task<NotificationRuleDb?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.NotificationRules
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
    }

    public async Task<IEnumerable<NotificationRuleDb>> GetByUserAsync(string userId, Guid? listId, CancellationToken cancellationToken)
    {
        var query = context.NotificationRules
            .Where(x => x.UserId == userId && !x.IsDeleted);
        
        if (listId.HasValue)
            query = query.Where(x => x.ListId == listId.Value);
        
        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<NotificationRuleDb>> GetActiveRulesAsync(Guid listId, TriggerType? triggerType, CancellationToken cancellationToken)
    {
        var query = context.NotificationRules
            .Where(x => x.ListId == listId && x.IsActive && !x.IsDeleted);
        
        if (triggerType.HasValue)
        {
            var rules = await query.ToListAsync(cancellationToken);
            return rules.Where(r =>
            {
                var trigger = JsonSerializer.Deserialize<NotificationTrigger>(r.TriggerJson);
                return trigger?.Type == triggerType.Value;
            });
        }
        
        return await query.ToListAsync(cancellationToken);
    }

    public Task CreateAsync(NotificationRuleDb rule, CancellationToken cancellationToken)
    {
        context.NotificationRules.Add(rule);
        
        var historyEntry = new NotificationRuleHistoryEntryDb
        {
            NotificationRuleId = rule.Id,
            Type = NotificationHistoryType.Created,
            On = DateTime.UtcNow,
            By = rule.CreatedBy
        };
        
        rule.History.Add(historyEntry);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(NotificationRuleDb rule, string updatedBy, CancellationToken cancellationToken)
    {
        rule.UpdatedOn = DateTime.UtcNow;
        rule.UpdatedBy = updatedBy;
        
        var historyEntry = new NotificationRuleHistoryEntryDb
        {
            NotificationRuleId = rule.Id,
            Type = NotificationHistoryType.Updated,
            On = DateTime.UtcNow,
            By = updatedBy
        };
        
        rule.History.Add(historyEntry);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(NotificationRuleDb rule, string deletedBy, CancellationToken cancellationToken)
    {
        rule.IsDeleted = true;
        rule.UpdatedOn = DateTime.UtcNow;
        rule.UpdatedBy = deletedBy;
        
        var historyEntry = new NotificationRuleHistoryEntryDb
        {
            NotificationRuleId = rule.Id,
            Type = NotificationHistoryType.Cancelled,
            On = DateTime.UtcNow,
            By = deletedBy
        };
        
        rule.History.Add(historyEntry);
        return Task.CompletedTask;
    }

    public Task SetTriggerAsync(NotificationRuleDb rule, NotificationTrigger trigger, CancellationToken cancellationToken)
    {
        rule.TriggerJson = JsonSerializer.Serialize(trigger);
        return Task.CompletedTask;
    }

    public Task<NotificationTrigger> GetTriggerAsync(NotificationRuleDb rule, CancellationToken cancellationToken)
    {
        return Task.FromResult(JsonSerializer.Deserialize<NotificationTrigger>(rule.TriggerJson) 
                               ?? throw new InvalidOperationException("Invalid trigger JSON"));
    }

    public Task SetChannelsAsync(NotificationRuleDb rule, NotificationChannel[] channels, CancellationToken cancellationToken)
    {
        rule.ChannelsJson = JsonSerializer.Serialize(channels);
        return Task.CompletedTask;
    }

    public Task<NotificationChannel[]> GetChannelsAsync(NotificationRuleDb rule, CancellationToken cancellationToken)
    {
        return Task.FromResult(JsonSerializer.Deserialize<NotificationChannel[]>(rule.ChannelsJson) ?? []);
    }

    public Task SetScheduleAsync(NotificationRuleDb rule, NotificationSchedule schedule, CancellationToken cancellationToken)
    {
        rule.ScheduleJson = JsonSerializer.Serialize(schedule);
        return Task.CompletedTask;
    }

    public Task<NotificationSchedule> GetScheduleAsync(NotificationRuleDb rule, CancellationToken cancellationToken)
    {
        return Task.FromResult(JsonSerializer.Deserialize<NotificationSchedule>(rule.ScheduleJson) 
                               ?? throw new InvalidOperationException("Invalid schedule JSON"));
    }

    public Task SetTemplateAsync(NotificationRuleDb rule, string templateId, CancellationToken cancellationToken)
    {
        rule.TemplateId = templateId;
        return Task.CompletedTask;
    }

    public Task SetActiveStatusAsync(NotificationRuleDb rule, bool isActive, CancellationToken cancellationToken)
    {
        rule.IsActive = isActive;
        return Task.CompletedTask;
    }
}