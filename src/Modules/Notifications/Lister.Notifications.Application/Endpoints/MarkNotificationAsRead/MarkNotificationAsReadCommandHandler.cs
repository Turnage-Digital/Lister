using Lister.Notifications.Domain;
using Lister.Notifications.Domain.Entities;
using MediatR;

namespace Lister.Notifications.Application.Endpoints.MarkNotificationAsRead;

public class MarkNotificationAsReadCommandHandler<TRule, TNotification>(
    NotificationAggregate<TRule, TNotification> aggregate
) : IRequestHandler<MarkNotificationAsReadCommand>
    where TRule : IWritableNotificationRule
    where TNotification : IWritableNotification
{
    public async Task Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
    {
        var notification = await aggregate.GetNotificationByIdAsync(
            request.NotificationId,
            request.UserId,
            cancellationToken);

        if (notification == null)
        {
            throw new InvalidOperationException(
                $"Notification {request.NotificationId} not found for user {request.UserId}");
        }

        // if (notification.ReadOn.HasValue)
        // {
        //     return; // Already read
        // }

        await aggregate.MarkNotificationAsReadAsync(
            notification,
            DateTime.UtcNow,
            cancellationToken);
    }
}