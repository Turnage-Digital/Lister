using System.Text.Json;
using Lister.Notifications.Domain.Queries;
using Lister.Notifications.Domain.ValueObjects;
using Lister.Notifications.Domain.Views;
using Microsoft.EntityFrameworkCore;

namespace Lister.Notifications.Infrastructure.Sql.Services;

public class UserNotificationsGetter(NotificationsDbContext context)
    : IGetUserNotifications
{
    public async Task<NotificationListPage> GetAsync(
        string userId,
        DateTime? since,
        Guid? listId,
        bool? unread,
        int pageSize,
        int page,
        CancellationToken cancellationToken
    )
    {
        var query = context.Notifications.AsQueryable().Where(n => n.UserId == userId);

        if (since.HasValue)
        {
            query = query.Where(n => n.CreatedOn >= since.Value);
        }

        if (listId.HasValue)
        {
            query = query.Where(n => n.ListId == listId.Value);
        }

        if (unread.HasValue)
        {
            query = unread.Value ? query.Where(n => n.ReadOn == null) : query.Where(n => n.ReadOn != null);
        }

        var total = await query.CountAsync(cancellationToken);
        var unreadCount = await context.Notifications
            .Where(n => n.UserId == userId && n.ReadOn == null)
            .CountAsync(cancellationToken);

        var pageRows = await query
            .OrderByDescending(n => n.CreatedOn)
            .Skip(page * pageSize)
            .Take(pageSize)
            .Select(n => new
            {
                n.Id,
                n.ListId,
                n.ItemId,
                n.ContentJson,
                n.ReadOn,
                n.CreatedOn
            })
            .ToListAsync(cancellationToken);

        var items = pageRows.Select(r =>
            {
                var title = string.Empty;
                var body = string.Empty;
                object? metadata = null;
                var occurredOn = r.CreatedOn;
                if (!string.IsNullOrEmpty(r.ContentJson))
                {
                    var content = JsonSerializer.Deserialize<NotificationContent>(r.ContentJson);
                    if (content is not null)
                    {
                        title = content.Subject;
                        body = content.Body;
                        if (content.Data is { Count: > 0 })
                        {
                            metadata = content.Data;
                        }

                        if (content.OccurredOn != default)
                        {
                            occurredOn = content.OccurredOn;
                        }
                    }
                }

                return new NotificationSummary
                {
                    Id = r.Id!.Value,
                    ListId = r.ListId,
                    ItemId = r.ItemId,
                    Title = title,
                    Body = body,
                    Metadata = metadata,
                    IsRead = r.ReadOn.HasValue,
                    OccurredOn = occurredOn
                };
            })
            .ToList();

        var retval = new NotificationListPage
        {
            Notifications = items,
            TotalCount = total,
            UnreadCount = unreadCount,
            HasMore = (page + 1) * pageSize < total
        };
        return retval;
    }
}