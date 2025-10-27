using Lister.Notifications.Domain.Entities;
using Lister.Notifications.ReadOnly.Dtos;
using Lister.Notifications.ReadOnly.Queries;
using MediatR;

namespace Lister.Notifications.Application.Endpoints.GetNotificationDetails;

public class GetNotificationDetailsQueryHandler<TRule, TNotification>(
    IGetNotificationDetails getter
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
        var dto = await getter.GetAsync(request.UserId!, request.NotificationId, cancellationToken);
        return dto;
    }
}
