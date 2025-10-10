using Lister.Core.Domain;
using Lister.Notifications.Domain;
using Lister.Notifications.Domain.Entities;
using Lister.Notifications.Domain.Enums;
using Lister.Notifications.Domain.ValueObjects;
using Moq;
using INotification = MediatR.INotification;

namespace Lister.Notifications.Tests;

[TestFixture]
public class NotificationAggregateTriggerTests
{
    [SetUp]
    public void SetUp()
    {
        _uow = new Mock<INotificationsUnitOfWork<IWritableNotificationRule, IWritableNotification>>();
        _queue = new TestQueue();
        _aggregate = new NotificationAggregate<IWritableNotificationRule, IWritableNotification>(_uow.Object, _queue);
    }

    private Mock<INotificationsUnitOfWork<IWritableNotificationRule, IWritableNotification>> _uow = null!;
    private NotificationAggregate<IWritableNotificationRule, IWritableNotification> _aggregate = null!;
    private IDomainEventQueue _queue = null!;
    private readonly Mock<IWritableNotificationRule> _rule = new();

    [Test]
    public async Task Simple_Type_Match_Returns_True()
    {
        var trigger = NotificationTrigger.ItemCreated();
        MockRulesTrigger(trigger);
        var result = await _aggregate.ShouldTriggerNotificationAsync(_rule.Object,
            NotificationTrigger.ItemCreated(), new Dictionary<string, object>());
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task ItemUpdated_Trigger_Matches()
    {
        var trigger = NotificationTrigger.ItemUpdated();
        MockRulesTrigger(trigger);
        var result = await _aggregate.ShouldTriggerNotificationAsync(_rule.Object,
            NotificationTrigger.ItemUpdated(), new Dictionary<string, object>());
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task Type_Mismatch_Returns_False()
    {
        MockRulesTrigger(NotificationTrigger.ItemDeleted());
        var result = await _aggregate.ShouldTriggerNotificationAsync(_rule.Object,
            NotificationTrigger.ItemCreated(), new Dictionary<string, object>());
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task StatusChanged_From_To_Must_Match_When_Specified()
    {
        MockRulesTrigger(NotificationTrigger.StatusChanged("Open", "Closed"));
        var ok = await _aggregate.ShouldTriggerNotificationAsync(_rule.Object,
            new NotificationTrigger { Type = TriggerType.StatusChanged, FromValue = "Open", ToValue = "Closed" },
            new Dictionary<string, object>());
        var badFrom = await _aggregate.ShouldTriggerNotificationAsync(_rule.Object,
            new NotificationTrigger { Type = TriggerType.StatusChanged, FromValue = "New", ToValue = "Closed" },
            new Dictionary<string, object>());
        var badTo = await _aggregate.ShouldTriggerNotificationAsync(_rule.Object,
            new NotificationTrigger { Type = TriggerType.StatusChanged, FromValue = "Open", ToValue = "Done" },
            new Dictionary<string, object>());

        using (Assert.EnterMultipleScope())
        {
            Assert.That(ok, Is.True);
            Assert.That(badFrom, Is.False);
            Assert.That(badTo, Is.False);
        }
    }

    [Test]
    public async Task CustomCondition_String_Operators_Work()
    {
        var ruleTrigger = new NotificationTrigger
        {
            Type = TriggerType.CustomCondition,
            ColumnName = "title",
            Operator = "contains",
            Value = "hello"
        };
        MockRulesTrigger(ruleTrigger);

        var ctx = new Dictionary<string, object> { ["title"] = "Well, hello there" };
        var actual = new NotificationTrigger { Type = TriggerType.CustomCondition };
        var result = await _aggregate.ShouldTriggerNotificationAsync(_rule.Object, actual, ctx);
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task CustomCondition_Numeric_Compare_Works()
    {
        var ruleTrigger = new NotificationTrigger
        {
            Type = TriggerType.CustomCondition,
            ColumnName = "count",
            Operator = ">",
            Value = "10"
        };
        MockRulesTrigger(ruleTrigger);
        var ctx = new Dictionary<string, object> { ["count"] = 15 };
        var actual = new NotificationTrigger { Type = TriggerType.CustomCondition };
        var result = await _aggregate.ShouldTriggerNotificationAsync(_rule.Object, actual, ctx);
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task CustomCondition_Regex_Works()
    {
        var ruleTrigger = new NotificationTrigger
        {
            Type = TriggerType.CustomCondition,
            ColumnName = "email",
            Operator = "regex",
            Value = @"^[^@]+@[^@]+\.[^@]+$"
        };
        MockRulesTrigger(ruleTrigger);
        var ctx = new Dictionary<string, object> { ["email"] = "user@example.com" };
        var actual = new NotificationTrigger { Type = TriggerType.CustomCondition };
        var result = await _aggregate.ShouldTriggerNotificationAsync(_rule.Object, actual, ctx);
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task CustomCondition_IsNull_IsNotNull_Work()
    {
        var isNullTrigger = new NotificationTrigger
        {
            Type = TriggerType.CustomCondition,
            ColumnName = "note",
            Operator = "isnull"
        };
        MockRulesTrigger(isNullTrigger);
        var ctx1 = new Dictionary<string, object>();
        var actual = new NotificationTrigger { Type = TriggerType.CustomCondition };
        var result1 = await _aggregate.ShouldTriggerNotificationAsync(_rule.Object, actual, ctx1);
        Assert.That(result1, Is.False); // key missing -> treated as not matching

        var ctx2 = new Dictionary<string, object> { ["note"] = null! };
        var result2 = await _aggregate.ShouldTriggerNotificationAsync(_rule.Object, actual, ctx2);
        Assert.That(result2, Is.True);

        var isNotNullTrigger = isNullTrigger with { Operator = "isnotnull" };
        MockRulesTrigger(isNotNullTrigger);
        var result3 = await _aggregate.ShouldTriggerNotificationAsync(_rule.Object, actual, ctx2);
        Assert.That(result3, Is.False);
    }

    private void MockRulesTrigger(NotificationTrigger trigger)
    {
        var rulesStore = new Mock<INotificationRulesStore<IWritableNotificationRule>>();
        rulesStore.Setup(s => s.GetTriggerAsync(_rule.Object, It.IsAny<CancellationToken>()))
            .ReturnsAsync(trigger);
        _uow.SetupGet(x => x.RulesStore).Returns(rulesStore.Object);
    }

    private class TestQueue : IDomainEventQueue
    {
        public void Enqueue(INotification @event, EventPhase phase)
        {
        }

        public IReadOnlyCollection<INotification> Dequeue(EventPhase phase)
        {
            return [];
        }
    }
}
