using Lister.Notifications.Domain.Entities;
using Lister.Notifications.ReadOnly.Queries;
using MediatR;

namespace Lister.Notifications.Application.Endpoints.Queries.GetUnreadNotificationCount;

public class GetUnreadNotificationCountQueryHandler<TRule, TNotification>(
    IGetUnreadNotificationCount getter
) : IRequestHandler<GetUnreadNotificationCountQuery, int>
    where TRule : IWritableNotificationRule
    where TNotification : IWritableNotification
{
    public async Task<int> Handle(GetUnreadNotificationCountQuery request, CancellationToken cancellationToken)
    {
        var retval = await getter
            .GetAsync(request.UserId!, request.ListId, cancellationToken);
        return retval;
    }
}