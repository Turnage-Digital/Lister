using Lister.Notifications.Domain;
using Lister.Notifications.Domain.Entities;
using Lister.Notifications.Domain.Enums;
using Lister.Notifications.Domain.Services;
using Lister.Notifications.Domain.ValueObjects;
using Moq;

namespace Lister.Notifications.Tests;

[TestFixture]
public class NotificationTriggerEvaluatorTests
{
    [SetUp]
    public void SetUp()
    {
        _evaluator = new NotificationTriggerEvaluator();
        _rulesStore = new Mock<INotificationRulesStore<IWritableNotificationRule>>();
        _rule = new Mock<IWritableNotificationRule>();
    }

    private NotificationTriggerEvaluator _evaluator = null!;
    private Mock<INotificationRulesStore<IWritableNotificationRule>> _rulesStore = null!;
    private Mock<IWritableNotificationRule> _rule = null!;

    [Test]
    public async Task Simple_Type_Match_Returns_True()
    {
        MockRuleTrigger(NotificationTrigger.ItemCreated());

        var result = await _evaluator.ShouldTriggerAsync(
            _rulesStore.Object,
            _rule.Object,
            NotificationTrigger.ItemCreated(),
            new Dictionary<string, object>(),
            CancellationToken.None);

        Assert.That(result, Is.True);
    }

    [Test]
    public async Task Type_Mismatch_Returns_False()
    {
        MockRuleTrigger(NotificationTrigger.ItemDeleted());

        var result = await _evaluator.ShouldTriggerAsync(
            _rulesStore.Object,
            _rule.Object,
            NotificationTrigger.ItemCreated(),
            new Dictionary<string, object>(),
            CancellationToken.None);

        Assert.That(result, Is.False);
    }

    [Test]
    public async Task StatusChanged_From_And_To_Must_Match()
    {
        MockRuleTrigger(NotificationTrigger.StatusChanged("Open", "Closed"));

        var ok = await _evaluator.ShouldTriggerAsync(
            _rulesStore.Object,
            _rule.Object,
            new NotificationTrigger { Type = TriggerType.StatusChanged, FromValue = "Open", ToValue = "Closed" },
            new Dictionary<string, object>(),
            CancellationToken.None);

        var wrongFrom = await _evaluator.ShouldTriggerAsync(
            _rulesStore.Object,
            _rule.Object,
            new NotificationTrigger { Type = TriggerType.StatusChanged, FromValue = "Draft", ToValue = "Closed" },
            new Dictionary<string, object>(),
            CancellationToken.None);

        var wrongTo = await _evaluator.ShouldTriggerAsync(
            _rulesStore.Object,
            _rule.Object,
            new NotificationTrigger { Type = TriggerType.StatusChanged, FromValue = "Open", ToValue = "Done" },
            new Dictionary<string, object>(),
            CancellationToken.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(ok, Is.True);
            Assert.That(wrongFrom, Is.False);
            Assert.That(wrongTo, Is.False);
        }
    }

    [Test]
    public async Task ColumnValueChanged_Requires_Matching_Column_And_Values()
    {
        var ruleTrigger = new NotificationTrigger
        {
            Type = TriggerType.ColumnValueChanged,
            ColumnName = "title",
            FromValue = "draft",
            ToValue = "published"
        };
        MockRuleTrigger(ruleTrigger);

        var actual = new NotificationTrigger
        {
            Type = TriggerType.ColumnValueChanged,
            ColumnName = "title",
            FromValue = "draft",
            ToValue = "published"
        };

        var result = await _evaluator.ShouldTriggerAsync(
            _rulesStore.Object,
            _rule.Object,
            actual,
            new Dictionary<string, object>(),
            CancellationToken.None);

        var differentColumn = await _evaluator.ShouldTriggerAsync(
            _rulesStore.Object,
            _rule.Object,
            actual with { ColumnName = "status" },
            new Dictionary<string, object>(),
            CancellationToken.None);

        var wrongTo = await _evaluator.ShouldTriggerAsync(
            _rulesStore.Object,
            _rule.Object,
            actual with { ToValue = "archived" },
            new Dictionary<string, object>(),
            CancellationToken.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.True);
            Assert.That(differentColumn, Is.False);
            Assert.That(wrongTo, Is.False);
        }
    }

    [Test]
    public async Task CustomCondition_String_Operators_Work()
    {
        var trigger = new NotificationTrigger
        {
            Type = TriggerType.CustomCondition,
            ColumnName = "title",
            Operator = "contains",
            Value = "hello"
        };
        MockRuleTrigger(trigger);

        var ctx = new Dictionary<string, object> { ["title"] = "Well, hello there" };
        var actual = new NotificationTrigger { Type = TriggerType.CustomCondition };

        var result = await _evaluator.ShouldTriggerAsync(
            _rulesStore.Object,
            _rule.Object,
            actual,
            ctx,
            CancellationToken.None);

        Assert.That(result, Is.True);
    }

    [Test]
    public async Task CustomCondition_Missing_Field_Returns_False()
    {
        var trigger = new NotificationTrigger
        {
            Type = TriggerType.CustomCondition,
            ColumnName = "title",
            Operator = "contains",
            Value = "hello"
        };
        MockRuleTrigger(trigger);

        var actual = new NotificationTrigger { Type = TriggerType.CustomCondition };
        var result = await _evaluator.ShouldTriggerAsync(
            _rulesStore.Object,
            _rule.Object,
            actual,
            new Dictionary<string, object>(),
            CancellationToken.None);

        Assert.That(result, Is.False);
    }

    [Test]
    public async Task CustomCondition_Regex_Works()
    {
        var trigger = new NotificationTrigger
        {
            Type = TriggerType.CustomCondition,
            ColumnName = "email",
            Operator = "regex",
            Value = @"^[^@]+@[^@]+\.[^@]+$"
        };
        MockRuleTrigger(trigger);

        var ctx = new Dictionary<string, object> { ["email"] = "user@example.com" };
        var actual = new NotificationTrigger { Type = TriggerType.CustomCondition };

        var result = await _evaluator.ShouldTriggerAsync(
            _rulesStore.Object,
            _rule.Object,
            actual,
            ctx,
            CancellationToken.None);

        Assert.That(result, Is.True);
    }

    [Test]
    public async Task CustomCondition_IsNull_And_IsNotNull()
    {
        var isNull = new NotificationTrigger
        {
            Type = TriggerType.CustomCondition,
            ColumnName = "note",
            Operator = "isnull"
        };
        MockRuleTrigger(isNull);

        var actual = new NotificationTrigger { Type = TriggerType.CustomCondition };
        var missingField = await _evaluator.ShouldTriggerAsync(
            _rulesStore.Object,
            _rule.Object,
            actual,
            new Dictionary<string, object>(),
            CancellationToken.None);

        var nullField = await _evaluator.ShouldTriggerAsync(
            _rulesStore.Object,
            _rule.Object,
            actual,
            new Dictionary<string, object> { ["note"] = null! },
            CancellationToken.None);

        var isNotNull = isNull with { Operator = "isnotnull" };
        MockRuleTrigger(isNotNull);
        var notNullResult = await _evaluator.ShouldTriggerAsync(
            _rulesStore.Object,
            _rule.Object,
            actual,
            new Dictionary<string, object> { ["note"] = "value" },
            CancellationToken.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(missingField, Is.False);
            Assert.That(nullField, Is.True);
            Assert.That(notNullResult, Is.True);
        }
    }

    private void MockRuleTrigger(NotificationTrigger trigger)
    {
        _rulesStore.Setup(s => s.GetTriggerAsync(_rule.Object, It.IsAny<CancellationToken>()))
            .ReturnsAsync(trigger);
    }
}