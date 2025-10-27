using System.Linq;
using Lister.Notifications.Domain;
using Lister.Notifications.Domain.Entities;
using Lister.Notifications.ReadOnly.Dtos;
using MediatR;

namespace Lister.Notifications.Application.Endpoints.CreateNotificationRule;

public class CreateNotificationRuleCommandHandler<TNotificationRule, TNotification>(
    NotificationAggregate<TNotificationRule, TNotification> aggregate
) : IRequestHandler<CreateNotificationRuleCommand, NotificationRuleDto>
    where TNotificationRule : IWritableNotificationRule
    where TNotification : IWritableNotification
{
    public async Task<NotificationRuleDto> Handle(
        CreateNotificationRuleCommand request,
        CancellationToken cancellationToken
    )
    {
        var rule = await aggregate.CreateNotificationRuleAsync(
            request.UserId!,
            request.ListId,
            request.Trigger,
            request.Channels,
            request.Schedule,
            request.TemplateId,
            cancellationToken);

        // Build view from created rule
        // The concrete rule may store JSON strings; deserialize to typed value objects
        var trigger = request.Trigger;
        var channels = request.Channels;
        var schedule = request.Schedule;

        return new NotificationRuleDto
        {
            Id = rule.Id,
            UserId = request.UserId!,
            ListId = request.ListId,
            IsActive = true,
            TemplateId = request.TemplateId,
            Trigger = new NotificationTriggerDto
            {
                Type = trigger.Type,
                FromValue = trigger.FromValue,
                ToValue = trigger.ToValue,
                ColumnName = trigger.ColumnName,
                Operator = trigger.Operator,
                Value = trigger.Value
            },
            Channels = channels
                .Select(channel => new NotificationChannelDto
                {
                    Type = channel.Type,
                    Address = channel.Address,
                    Settings = channel.Settings
                })
                .ToArray(),
            Schedule = new NotificationScheduleDto
            {
                Type = schedule.Type,
                Delay = schedule.Delay,
                CronExpression = schedule.CronExpression,
                DailyAt = schedule.DailyAt,
                DaysOfWeek = schedule.DaysOfWeek
            }
        };
    }
}
