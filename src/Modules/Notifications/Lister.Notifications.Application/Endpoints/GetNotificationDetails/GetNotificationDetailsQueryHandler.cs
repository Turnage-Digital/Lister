using Lister.Notifications.Domain;
using Lister.Notifications.Domain.Entities;
using MediatR;

namespace Lister.Notifications.Application.Endpoints.GetNotificationDetails;

public class GetNotificationDetailsQueryHandler<TRule, TNotification>(
    NotificationAggregate<TRule, TNotification> aggregate
)
    : IRequestHandler<GetNotificationDetailsQuery, NotificationDetailsDto?>
    where TRule : IWritableNotificationRule
    where TNotification : IWritableNotification
{
    public async Task<NotificationDetailsDto?> Handle(
        GetNotificationDetailsQuery request,
        CancellationToken cancellationToken
    )
    {
        var notification = await aggregate.GetNotificationByIdAsync(
            request.NotificationId,
            request.UserId,
            cancellationToken);

        if (notification == null)
        {
            return null;
        }

        return new NotificationDetailsDto
        {
            Id = notification.Id ?? Guid.Empty,
            NotificationRuleId = notification.NotificationRuleId,
            UserId = notification.UserId,
            ListId = notification.ListId,
            ItemId = notification.ItemId,
            Title = "Notification", // TODO: Extract from content
            Body = "Notification body", // TODO: Extract from content
            // CreatedOn = notification.CreatedOn,
            // ProcessedOn = notification.ProcessedOn,
            // DeliveredOn = notification.DeliveredOn,
            // ReadOn = notification.ReadOn,
            Metadata = null, // TODO: Add metadata from content
            DeliveryAttempts = [] // TODO: Load delivery attempts
        };
    }
}