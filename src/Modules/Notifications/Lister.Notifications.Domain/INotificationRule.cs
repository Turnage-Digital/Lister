namespace Lister.Notifications.Domain;

public interface INotificationRule
{
    Guid? Id { get; }
    string UserId { get; }
    Guid ListId { get; }
    // bool IsActive { get; }
    // string? TemplateId { get; }
    // DateTime CreatedOn { get; }
    // string CreatedBy { get; }
    // DateTime? UpdatedOn { get; }
    // string? UpdatedBy { get; }
    // string TriggerJson { get; }
    // string ChannelsJson { get; }
    // string ScheduleJson { get; }
}