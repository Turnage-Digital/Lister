using Lister.Notifications.Application.Mappings;
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
            request.IsActive,
            cancellationToken);

        var trigger = await aggregate.GetNotificationRuleTriggerAsync(rule, cancellationToken);
        var channels = await aggregate.GetNotificationRuleChannelsAsync(rule, cancellationToken);
        var schedule = await aggregate.GetNotificationRuleScheduleAsync(rule, cancellationToken);
        var templateId = await aggregate.GetNotificationRuleTemplateAsync(rule, cancellationToken);
        var isActive = await aggregate.GetNotificationRuleIsActiveAsync(rule, cancellationToken);

        return NotificationRuleWriteContextMap.ToDto(
            rule,
            trigger,
            channels,
            schedule,
            templateId,
            isActive);
    }
}
