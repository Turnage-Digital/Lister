using Lister.Notifications.Domain;
using Lister.Notifications.Domain.Entities;
using MediatR;

namespace Lister.Notifications.Application.Endpoints.GetUnreadNotificationCount;

public class GetUnreadNotificationCountQueryHandler<TRule, TNotification> : IRequestHandler<GetUnreadNotificationCountQuery, int>
    where TRule : IWritableNotificationRule
    where TNotification : IWritableNotification
{
    private readonly NotificationAggregate<TRule, TNotification> _aggregate;

    public GetUnreadNotificationCountQueryHandler(NotificationAggregate<TRule, TNotification> aggregate)
    {
        _aggregate = aggregate;
    }

    public async Task<int> Handle(GetUnreadNotificationCountQuery request, CancellationToken cancellationToken)
    {
        return await _aggregate.GetUnreadCountAsync(
            request.UserId,
            request.ListId,
            cancellationToken);
    }
}