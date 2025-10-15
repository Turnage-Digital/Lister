using Lister.Notifications.Domain.Entities;
using Lister.Notifications.Domain.Queries;
using Lister.Notifications.Domain.Views;
using MediatR;

namespace Lister.Notifications.Application.Endpoints.GetNotificationDetails;

public class GetNotificationDetailsQueryHandler<TRule, TNotification>(
    IGetNotificationDetails getter
)
    : IRequestHandler<GetNotificationDetailsQuery, NotificationDetails?>
    where TRule : IWritableNotificationRule
    where TNotification : IWritableNotification
{
    public async Task<NotificationDetails?> Handle(
        GetNotificationDetailsQuery request,
        CancellationToken cancellationToken
    )
    {
        var dto = await getter.GetAsync(request.UserId!, request.NotificationId, cancellationToken);
        return dto;
    }
}