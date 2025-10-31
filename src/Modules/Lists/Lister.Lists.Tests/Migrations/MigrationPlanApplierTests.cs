using Lister.Lists.Domain.Enums;
using Lister.Lists.Domain.Services;
using Lister.Lists.Domain.ValueObjects;

namespace Lister.Lists.Tests.Migrations;

[TestFixture]
public class MigrationPlanApplierTests
{
    [Test]
    public void Prepare_RenameStorageKeys_UpdatesColumns()
    {
        var plan = new MigrationPlan
        {
            RenameStorageKeys = [new RenameStorageKeyOp("prop1", "propMain")]
        };

        var columns = new[]
        {
            new Column { StorageKey = "prop1", Name = "Title", Type = ColumnType.Text }
        };
        var statuses = Array.Empty<Status>();
        var transitions = Array.Empty<StatusTransition>();

        var context = MigrationPlanApplier.Prepare(plan, columns, statuses, transitions);

        Assert.That(context.Columns.Single().StorageKey, Is.EqualTo("propMain"));
    }

    [Test]
    public void Prepare_ChangeColumnTypes_UpdatesMetadata()
    {
        var plan = new MigrationPlan
        {
            ChangeColumnTypes = [new ChangeColumnTypeOp("prop1", ColumnType.Number, "parse")]
        };

        var columns = new[]
        {
            new Column { StorageKey = "prop1", Name = "Total", Type = ColumnType.Text }
        };

        var context =
            MigrationPlanApplier.Prepare(plan, columns, [], []);

        Assert.That(context.Columns.Single().Type, Is.EqualTo(ColumnType.Number));
        Assert.That(context.ColumnTypeChanges.Single().Key, Is.EqualTo("prop1"));
    }

    [Test]
    public void ApplyToItem_ChangeColumnTypes_ConvertsValues()
    {
        var plan = new MigrationPlan
        {
            ChangeColumnTypes = [new ChangeColumnTypeOp("prop1", ColumnType.Number, "parseNumber")]
        };
        var context = MigrationPlanApplier.Prepare(
            plan,
            [new Column { StorageKey = "prop1", Name = "Total", Type = ColumnType.Text }],
            [],
            []);

        var bag = MigrationPlanApplier.ApplyToItem(context, new Dictionary<string, object?> { ["prop1"] = "42" });

        Assert.That(bag["prop1"], Is.TypeOf<decimal>());
    }

    [Test]
    public void ApplyToItem_RemoveColumns_DropsEntries()
    {
        var plan = new MigrationPlan
        {
            RemoveColumns = [new RemoveColumnOp("prop2", "drop")]
        };
        var context = MigrationPlanApplier.Prepare(
            plan,
            [
                new Column { StorageKey = "prop1", Name = "Title", Type = ColumnType.Text, Required = true },
                new Column { StorageKey = "prop2", Name = "Obsolete", Type = ColumnType.Text, Required = false }
            ],
            [],
            []);

        var bag = MigrationPlanApplier.ApplyToItem(context,
            new Dictionary<string, object?> { ["prop1"] = "keep", ["prop2"] = "remove" });

        Assert.That(bag.ContainsKey("prop2"), Is.False);
        Assert.That(context.Columns.Any(c => c.StorageKey == "prop2"), Is.False);
    }

    [Test]
    public void ApplyToItem_RemoveStatuses_MapsStatusValue()
    {
        var plan = new MigrationPlan
        {
            RemoveStatuses = [new RemoveStatusOp("Active", "Closed")]
        };
        var context = MigrationPlanApplier.Prepare(
            plan,
            [],
            [new Status { Name = "Active", Color = "green" }, new Status { Name = "Closed", Color = "gray" }],
            [
                new StatusTransition { From = "Active", AllowedNext = ["Closed"] },
                new StatusTransition { From = "Closed", AllowedNext = [] }
            ]);

        var bag = MigrationPlanApplier.ApplyToItem(context,
            new Dictionary<string, object?> { ["status"] = "Active" });

        Assert.That(bag["status"], Is.EqualTo("Closed"));
        Assert.That(context.Statuses.Any(s => s.Name == "Active"), Is.False);
        Assert.That(context.StatusTransitions.Any(t => t.From == "Active"), Is.False);
    }
}