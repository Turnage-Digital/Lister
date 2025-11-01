using Lister.Notifications.Domain;
using Lister.Notifications.Domain.Entities;
using MediatR;

namespace Lister.Notifications.Application.Commands.MarkNotificationAsRead;

public class MarkNotificationAsReadCommandHandler<TRule, TNotification>(
    NotificationAggregate<TRule, TNotification> aggregate
) : IRequestHandler<MarkNotificationAsReadCommand>
    where TRule : IWritableNotificationRule
    where TNotification : IWritableNotification
{
    public async Task Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.UserId))
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }

        var notification = await aggregate.GetNotificationByIdAsync(
            request.NotificationId,
            request.UserId!,
            cancellationToken);

        if (notification is null)
        {
            throw new InvalidOperationException(
                $"Notification {request.NotificationId} not found for user {request.UserId}");
        }

        await aggregate.MarkNotificationAsReadAsync(
            notification,
            DateTime.UtcNow,
            cancellationToken);
    }
}