using Lister.Core.Domain.ValueObjects;
using Lister.Notifications.Domain.Enums;
using Lister.Notifications.Infrastructure.Sql.Entities;

namespace Lister.Notifications.Infrastructure.Sql.ValueObjects;

public record NotificationRuleHistoryEntryDb : Entry<NotificationHistoryType>
{
    public int? Id { get; set; }
    public Guid? NotificationRuleId { get; set; }
    public NotificationRuleDb? NotificationRule { get; set; }
}