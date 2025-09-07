using Lister.Core.Domain.ValueObjects;
using Lister.Notifications.Domain.Enums;
using Lister.Notifications.Infrastructure.Sql.Entities;

namespace Lister.Notifications.Infrastructure.Sql.ValueObjects;

public record NotificationHistoryEntryDb : Entry<NotificationHistoryType>
{
    public int? Id { get; set; }
    public Guid? NotificationId { get; set; }
    public NotificationDb? Notification { get; set; }
}