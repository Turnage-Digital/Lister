using System.Text.Json;
using Lister.Core.Infrastructure.Sql;
using Lister.Core.Infrastructure.Sql.Entities;
using Lister.Notifications.Domain.Events;
using MediatR;

namespace Lister.App.Server.Integration;

public class NotificationOutboxBase(CoreDbContext db)
{
    protected async Task EnqueueAsync<T>(T evt, CancellationToken ct)
    {
        var msg = new OutboxMessageDb
        {
            Type = typeof(T).FullName ?? typeof(T).Name,
            PayloadJson = JsonSerializer.Serialize(evt),
            CreatedOn = DateTime.UtcNow
        };
        db.OutboxMessages.Add(msg);
        await db.SaveChangesAsync(ct);
    }
}

public class NotificationCreatedOutboxHandler(CoreDbContext db)
    : NotificationOutboxBase(db), INotificationHandler<NotificationCreatedEvent>
{
    public Task Handle(NotificationCreatedEvent notification, CancellationToken cancellationToken)
    {
        return EnqueueAsync(notification, cancellationToken);
    }
}

public class NotificationProcessedOutboxHandler(CoreDbContext db)
    : NotificationOutboxBase(db), INotificationHandler<NotificationProcessedEvent>
{
    public Task Handle(NotificationProcessedEvent notification, CancellationToken cancellationToken)
    {
        return EnqueueAsync(notification, cancellationToken);
    }
}

public class NotificationReadOutboxHandler(CoreDbContext db)
    : NotificationOutboxBase(db), INotificationHandler<NotificationReadEvent>
{
    public Task Handle(NotificationReadEvent notification, CancellationToken cancellationToken)
    {
        return EnqueueAsync(notification, cancellationToken);
    }
}

public class AllNotificationsReadOutboxHandler(CoreDbContext db)
    : NotificationOutboxBase(db), INotificationHandler<AllNotificationsReadEvent>
{
    public Task Handle(AllNotificationsReadEvent notification, CancellationToken cancellationToken)
    {
        return EnqueueAsync(notification, cancellationToken);
    }
}

public class NotificationDeliveryAttemptedOutboxHandler(CoreDbContext db)
    : NotificationOutboxBase(db), INotificationHandler<NotificationDeliveryAttemptedEvent>
{
    public Task Handle(NotificationDeliveryAttemptedEvent notification, CancellationToken cancellationToken)
    {
        return EnqueueAsync(notification, cancellationToken);
    }
}

public class NotificationRuleCreatedOutboxHandler(CoreDbContext db)
    : NotificationOutboxBase(db), INotificationHandler<NotificationRuleCreatedEvent>
{
    public Task Handle(NotificationRuleCreatedEvent notification, CancellationToken cancellationToken)
    {
        return EnqueueAsync(notification, cancellationToken);
    }
}

public class NotificationRuleUpdatedOutboxHandler(CoreDbContext db)
    : NotificationOutboxBase(db), INotificationHandler<NotificationRuleUpdatedEvent>
{
    public Task Handle(NotificationRuleUpdatedEvent notification, CancellationToken cancellationToken)
    {
        return EnqueueAsync(notification, cancellationToken);
    }
}

public class NotificationRuleDeletedOutboxHandler(CoreDbContext db)
    : NotificationOutboxBase(db), INotificationHandler<NotificationRuleDeletedEvent>
{
    public Task Handle(NotificationRuleDeletedEvent notification, CancellationToken cancellationToken)
    {
        return EnqueueAsync(notification, cancellationToken);
    }
}