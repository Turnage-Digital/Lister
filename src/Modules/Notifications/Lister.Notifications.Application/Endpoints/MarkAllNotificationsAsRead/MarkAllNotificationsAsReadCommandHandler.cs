using Lister.Notifications.Domain;
using Lister.Notifications.Domain.Entities;
using MediatR;

namespace Lister.Notifications.Application.Endpoints.MarkAllNotificationsAsRead;

public class MarkAllNotificationsAsReadCommandHandler<TRule, TNotification> : IRequestHandler<MarkAllNotificationsAsReadCommand>
    where TRule : IWritableNotificationRule
    where TNotification : IWritableNotification
{
    private readonly NotificationAggregate<TRule, TNotification> _aggregate;

    public MarkAllNotificationsAsReadCommandHandler(NotificationAggregate<TRule, TNotification> aggregate)
    {
        _aggregate = aggregate;
    }

    public async Task Handle(MarkAllNotificationsAsReadCommand request, CancellationToken cancellationToken)
    {
        await _aggregate.MarkAllNotificationsAsReadAsync(
            request.UserId,
            DateTime.UtcNow,
            request.Before,
            cancellationToken);
    }
}