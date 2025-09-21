using MediatR;

namespace Lister.Notifications.Application.Endpoints.GetUnreadNotificationCount;

public class GetUnreadNotificationCountQuery : IRequest<int>
{
    public string UserId { get; set; } = string.Empty;
    public Guid? ListId { get; set; }
}