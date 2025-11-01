using Lister.Notifications.Domain;
using Lister.Notifications.Domain.Entities;
using MediatR;

namespace Lister.Notifications.Application.Commands.MarkAllNotificationsAsRead;

public class
    MarkAllNotificationsAsReadCommandHandler<TRule, TNotification>(
        NotificationAggregate<TRule, TNotification> aggregate
    ) : IRequestHandler<MarkAllNotificationsAsReadCommand>
    where TRule : IWritableNotificationRule
    where TNotification : IWritableNotification
{
    public async Task Handle(MarkAllNotificationsAsReadCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.UserId))
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }

        await aggregate.MarkAllNotificationsAsReadAsync(
            request.UserId!,
            DateTime.UtcNow,
            request.Before,
            cancellationToken);
    }
}