using Lister.Notifications.Domain;
using Lister.Notifications.Domain.Entities;
using MediatR;

namespace Lister.Notifications.Application.Endpoints.MarkNotificationAsRead;

public class MarkNotificationAsReadCommandHandler<TRule, TNotification> : IRequestHandler<MarkNotificationAsReadCommand>
    where TRule : IWritableNotificationRule
    where TNotification : IWritableNotification
{
    private readonly NotificationAggregate<TRule, TNotification> _aggregate;

    public MarkNotificationAsReadCommandHandler(NotificationAggregate<TRule, TNotification> aggregate)
    {
        _aggregate = aggregate;
    }

    public async Task Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
    {
        var notification = await _aggregate.GetNotificationByIdAsync(
            request.NotificationId,
            request.UserId,
            cancellationToken);

        if (notification == null)
            throw new InvalidOperationException($"Notification {request.NotificationId} not found for user {request.UserId}");

        if (notification.ReadOn.HasValue)
            return; // Already read

        await _aggregate.MarkNotificationAsReadAsync(
            notification,
            DateTime.UtcNow,
            cancellationToken);
    }
}