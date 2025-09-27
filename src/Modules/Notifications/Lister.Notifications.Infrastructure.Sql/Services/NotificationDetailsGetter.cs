using System.Text.Json;
using Lister.Core.Domain.ValueObjects;
using Lister.Notifications.Domain.Enums;
using Lister.Notifications.Domain.Services;
using Lister.Notifications.Domain.ValueObjects;
using Lister.Notifications.Domain.Views;
using Microsoft.EntityFrameworkCore;

namespace Lister.Notifications.Infrastructure.Sql.Services;

public class NotificationDetailsGetter(NotificationsDbContext context) : IGetNotificationDetails
{
    public async Task<NotificationDetails?> GetAsync(
        string userId,
        Guid notificationId,
        CancellationToken cancellationToken
    )
    {
        var n = await context.Notifications
            .AsNoTracking()
            .AsSplitQuery()
            .Include(x => x.History)
            .Include(x => x.DeliveryAttempts)
            .FirstOrDefaultAsync(x => x.Id == notificationId && x.UserId == userId, cancellationToken);

        if (n is null)
        {
            return null;
        }

        var title = string.Empty;
        var body = string.Empty;
        object? metadata = null;
        if (!string.IsNullOrEmpty(n.ContentJson))
        {
            var content = JsonSerializer.Deserialize<NotificationContent>(n.ContentJson);
            if (content is not null)
            {
                title = content.Subject;
                body = content.Body;
                metadata = content.Data;
            }
        }

        var history = n.History
            .OrderBy(h => h.On)
            .Select(h => new Entry<NotificationHistoryType>
            {
                On = h.On,
                By = h.By,
                Type = h.Type,
                Bag = h.Bag
            })
            .ToArray();

        var attempts = n.DeliveryAttempts
            .OrderBy(a => a.AttemptedOn)
            .Select(a => new DeliveryAttemptView
            {
                Channel = JsonSerializer
                    .Deserialize<NotificationChannel>(a.ChannelJson)
                    ?.Type.ToString() ?? "",
                AttemptedOn = a.AttemptedOn,
                Status = ((DeliveryStatus)a.Status).ToString(),
                FailureReason = a.FailureReason,
                AttemptNumber = a.AttemptNumber
            })
            .ToList();

        return new NotificationDetails
        {
            Id = n.Id!.Value,
            NotificationRuleId = n.NotificationRuleId,
            UserId = n.UserId,
            ListId = n.ListId,
            ItemId = n.ItemId,
            Title = title,
            Body = body,
            Metadata = metadata,
            IsRead = n.ReadOn.HasValue,
            History = history,
            DeliveryAttempts = attempts
        };
    }
}