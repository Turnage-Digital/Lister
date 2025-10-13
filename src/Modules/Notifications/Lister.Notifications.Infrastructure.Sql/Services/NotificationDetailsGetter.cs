using System.Text.Json;
using Lister.Core.Domain.ValueObjects;
using Lister.Notifications.Domain.Enums;
using Lister.Notifications.Domain.Services;
using Lister.Notifications.Domain.ValueObjects;
using Lister.Notifications.Domain.Views;
using Microsoft.EntityFrameworkCore;

namespace Lister.Notifications.Infrastructure.Sql.Services;

public class NotificationDetailsGetter(NotificationsDbContext context)
    : IGetNotificationDetails
{
    public async Task<NotificationDetails?> GetAsync(
        string userId,
        Guid notificationId,
        CancellationToken cancellationToken
    )
    {
        NotificationDetails? retval = null;

        var entity = await context.Notifications
            .AsNoTracking()
            .AsSplitQuery()
            .Include(n => n.History)
            .Include(n => n.DeliveryAttempts)
            .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId, cancellationToken);

        if (entity is not null)
        {
            var title = string.Empty;
            var body = string.Empty;
            object? metadata = null;
            if (!string.IsNullOrEmpty(entity.ContentJson))
            {
                var content = JsonSerializer.Deserialize<NotificationContent>(entity.ContentJson);
                if (content is not null)
                {
                    title = content.Subject;
                    body = content.Body;
                    metadata = content.Data;
                }
            }

            var history = entity.History
                .OrderBy(h => h.On)
                .Select(h => new Entry<NotificationHistoryType>
                {
                    On = h.On,
                    By = h.By,
                    Type = h.Type,
                    Bag = h.Bag
                })
                .ToArray();

            var attempts = entity.DeliveryAttempts
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

            retval = new NotificationDetails
            {
                Id = entity.Id!.Value,
                NotificationRuleId = entity.NotificationRuleId,
                UserId = entity.UserId,
                ListId = entity.ListId,
                ItemId = entity.ItemId,
                Title = title,
                Body = body,
                Metadata = metadata,
                IsRead = entity.ReadOn.HasValue,
                History = history,
                DeliveryAttempts = attempts
            };
        }

        return retval;
    }
}