using Lister.Core.Domain;
using Lister.Notifications.Domain;
using Lister.Notifications.Domain.Entities;
using Lister.Notifications.Domain.Enums;
using Lister.Notifications.Domain.Events;
using Lister.Notifications.Domain.ValueObjects;
using Moq;
using INotification = MediatR.INotification;

namespace Lister.Notifications.Tests;

[TestFixture]
public class NotificationAggregateEventTests
{
    private class TestQueue : IDomainEventQueue
    {
        public readonly List<INotification> Events = [];

        public void Enqueue(INotification @event, EventPhase phase)
        {
            Events.Add(@event);
        }

        public IReadOnlyCollection<INotification> Dequeue(EventPhase phase)
        {
            return Events.ToArray();
        }
    }

    private class FakeRule : IWritableNotificationRule
    {
        public Guid? Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public Guid ListId { get; set; }
    }

    private class FakeNotification : IWritableNotification
    {
        public Guid? Id { get; set; }
        public Guid? NotificationRuleId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public Guid ListId { get; set; }
        public int? ItemId { get; set; }
    }

    private readonly string _user = "user1";
    private readonly Guid _listId = Guid.NewGuid();

    private (NotificationAggregate<IWritableNotificationRule, IWritableNotification> agg,
        Mock<INotificationsUnitOfWork<IWritableNotificationRule, IWritableNotification>> uow,
        TestQueue queue,
        Mock<INotificationRulesStore<IWritableNotificationRule>> rulesStore,
        Mock<INotificationsStore<IWritableNotification>> notifStore) Create()
    {
        var uow = new Mock<INotificationsUnitOfWork<IWritableNotificationRule, IWritableNotification>>();
        var rules = new Mock<INotificationRulesStore<IWritableNotificationRule>>();
        var notifs = new Mock<INotificationsStore<IWritableNotification>>();
        uow.SetupGet(x => x.RulesStore).Returns(rules.Object);
        uow.SetupGet(x => x.NotificationsStore).Returns(notifs.Object);
        uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(0);
        var queue = new TestQueue();
        var agg = new NotificationAggregate<IWritableNotificationRule, IWritableNotification>(uow.Object, queue);
        return (agg, uow, queue, rules, notifs);
    }

    [Test]
    public async Task CreateNotificationRule_Enqueues_Event()
    {
        var (agg, _, queue, rules, _) = Create();
        var rule = new FakeRule { Id = Guid.NewGuid(), ListId = _listId, UserId = _user };
        rules.Setup(s => s.InitAsync(_user, _listId, It.IsAny<CancellationToken>())).ReturnsAsync(rule);
        rules.Setup(s => s.SetTriggerAsync(rule, It.IsAny<NotificationTrigger>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        rules.Setup(s => s.SetChannelsAsync(rule, It.IsAny<NotificationChannel[]>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        rules.Setup(s => s.SetScheduleAsync(rule, It.IsAny<NotificationSchedule>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        rules.Setup(s => s.CreateAsync(rule, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        await agg.CreateNotificationRuleAsync(
            _user,
            _listId,
            NotificationTrigger.ItemCreated(),
            [NotificationChannel.InApp()],
            new NotificationSchedule());

        Assert.That(queue.Events.OfType<NotificationRuleCreatedEvent>().Any(), Is.True);
    }

    [Test]
    public async Task UpdateNotificationRule_Enqueues_Event()
    {
        var (agg, _, queue, rules, _) = Create();
        var rule = new FakeRule { Id = Guid.NewGuid(), ListId = _listId, UserId = _user };
        rules.Setup(s => s.UpdateAsync(rule, _user, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        rules.Setup(s => s.SetActiveStatusAsync(rule, true, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        await agg.UpdateNotificationRuleAsync(rule, _user, isActive: true);
        Assert.That(queue.Events.OfType<NotificationRuleUpdatedEvent>().Any(), Is.True);
    }

    [Test]
    public async Task DeleteNotificationRule_Enqueues_Event()
    {
        var (agg, _, queue, rules, _) = Create();
        var rule = new FakeRule { Id = Guid.NewGuid(), ListId = _listId, UserId = _user };
        rules.Setup(s => s.DeleteAsync(rule, _user, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        await agg.DeleteNotificationRuleAsync(rule, _user);
        Assert.That(queue.Events.OfType<NotificationRuleDeletedEvent>().Any(), Is.True);
    }

    [Test]
    public async Task CreateNotification_Enqueues_Event()
    {
        var (agg, _, queue, _, notifs) = Create();
        var n = new FakeNotification { UserId = _user, ListId = _listId };
        notifs.Setup(s => s.InitAsync(_user, _listId, It.IsAny<CancellationToken>())).ReturnsAsync(n);
        notifs.Setup(s => s.SetContentAsync(n, It.IsAny<NotificationContent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        notifs.Setup(s => s.SetPriorityAsync(n, NotificationPriority.Normal, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        notifs.Setup(s => s.CreateAsync(n, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        await agg.CreateNotificationAsync(_user, _listId, null, null,
            new NotificationContent { Subject = "t", Body = "b" });
        Assert.That(queue.Events.OfType<NotificationCreatedEvent>().Any(), Is.True);
    }

    [Test]
    public async Task MarkProcessed_Enqueues_Event()
    {
        var (agg, _, queue, _, notifs) = Create();
        var n = new FakeNotification { UserId = _user, ListId = _listId };
        notifs.Setup(s => s.MarkAsProcessedAsync(n, It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await agg.MarkNotificationAsProcessedAsync(n);
        Assert.That(queue.Events.OfType<NotificationProcessedEvent>().Any(), Is.True);
    }

    [Test]
    public async Task RecordDeliveryAttempt_Enqueues_Event_And_Marks_Delivered_On_Success()
    {
        var (agg, _, queue, _, notifs) = Create();
        var n = new FakeNotification { UserId = _user, ListId = _listId };
        var email = NotificationChannel.Email("user@example.com");
        notifs.Setup(s => s.GetDeliveryAttemptCountAsync(n, email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        notifs.Setup(s =>
                s.AddDeliveryAttemptAsync(n, It.IsAny<NotificationDeliveryAttempt>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        notifs.Setup(s => s.MarkAsDeliveredAsync(n, It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await agg.RecordDeliveryAttemptAsync(n, email, DeliveryStatus.Delivered);

        Assert.That(queue.Events.OfType<NotificationDeliveryAttemptedEvent>().Any(), Is.True);
        notifs.Verify(s => s.MarkAsDeliveredAsync(n, It.IsAny<DateTime>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task MarkRead_And_AllRead_Enqueue_Events()
    {
        var (agg, _, queue, _, notifs) = Create();
        var n = new FakeNotification { UserId = _user, ListId = _listId };
        notifs.Setup(s => s.MarkAsReadAsync(n, It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        notifs.Setup(s => s.MarkAllAsReadAsync(_user, It.IsAny<DateTime>(), null, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await agg.MarkNotificationAsReadAsync(n, DateTime.UtcNow);
        await agg.MarkAllNotificationsAsReadAsync(_user, DateTime.UtcNow);

        Assert.That(queue.Events.OfType<NotificationReadEvent>().Any(), Is.True);
        Assert.That(queue.Events.OfType<AllNotificationsReadEvent>().Any(), Is.True);
    }
}