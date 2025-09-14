using MediatR;

namespace Lister.Notifications.Application.Endpoints.GetUserNotifications;

public class GetUserNotificationsQuery : IRequest<GetUserNotificationsResponse>
{
    public string UserId { get; set; } = string.Empty;
    public DateTime? Since { get; set; }
    public bool? Unread { get; set; }
    public Guid? ListId { get; set; }
    public int PageSize { get; set; } = 20;
    public int Page { get; set; } = 0;
}