using Lister.Notifications.Domain;
using Lister.Notifications.Domain.Entities;
using MediatR;

namespace Lister.Notifications.Application.Endpoints.GetNotificationDetails;

public class GetNotificationDetailsQueryHandler<TRule, TNotification> : IRequestHandler<GetNotificationDetailsQuery, NotificationDetailsDto?>
    where TRule : IWritableNotificationRule
    where TNotification : IWritableNotification
{
    private readonly NotificationAggregate<TRule, TNotification> _aggregate;

    public GetNotificationDetailsQueryHandler(NotificationAggregate<TRule, TNotification> aggregate)
    {
        _aggregate = aggregate;
    }

    public async Task<NotificationDetailsDto?> Handle(GetNotificationDetailsQuery request, CancellationToken cancellationToken)
    {
        var notification = await _aggregate.GetNotificationByIdAsync(
            request.NotificationId,
            request.UserId,
            cancellationToken);

        if (notification == null)
            return null;

        return new NotificationDetailsDto
        {
            Id = notification.Id ?? Guid.Empty,
            NotificationRuleId = notification.NotificationRuleId,
            UserId = notification.UserId,
            ListId = notification.ListId,
            ItemId = notification.ItemId,
            Title = "Notification", // TODO: Extract from content
            Body = "Notification body", // TODO: Extract from content
            CreatedOn = notification.CreatedOn,
            ProcessedOn = notification.ProcessedOn,
            DeliveredOn = notification.DeliveredOn,
            ReadOn = notification.ReadOn,
            Metadata = null, // TODO: Add metadata from content
            DeliveryAttempts = new() // TODO: Load delivery attempts
        };
    }
}