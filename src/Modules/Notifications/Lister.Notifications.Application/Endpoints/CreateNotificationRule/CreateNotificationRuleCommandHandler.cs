using Lister.Notifications.Domain;
using Lister.Notifications.Domain.Entities;
using Lister.Notifications.Domain.Views;
using MediatR;

namespace Lister.Notifications.Application.Endpoints.CreateNotificationRule;

public class CreateNotificationRuleCommandHandler<TNotificationRule, TNotification>(
    NotificationAggregate<TNotificationRule, TNotification> aggregate
) : IRequestHandler<CreateNotificationRuleCommand, NotificationRule>
    where TNotificationRule : IWritableNotificationRule
    where TNotification : IWritableNotification
{
    public async Task<NotificationRule> Handle(
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

        return new NotificationRule
        {
            Id = rule.Id,
            UserId = request.UserId!,
            ListId = request.ListId,
            IsActive = true,
            TemplateId = request.TemplateId,
            Trigger = trigger,
            Channels = channels,
            Schedule = schedule
        };
    }
}