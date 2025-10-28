using Lister.Lists.Application.Endpoints.Migrations;
using Lister.Lists.Domain.Enums;
using Lister.Lists.ReadOnly.Dtos;
using Lister.Lists.ReadOnly.Queries;
using Moq;

namespace Lister.Lists.Tests.Migrations;

[TestFixture]
public class MigrationValidatorTests
{
    [SetUp]
    public void SetUp()
    {
        _listId = Guid.NewGuid();
        _getter = new Mock<IGetListItemDefinition>();
        _validator = new MigrationValidator(_getter.Object);

        var def = new ListItemDefinitionDto
        {
            Id = _listId,
            Name = "Test",
            Columns =
            [
                new ColumnDto
                {
                    StorageKey = "prop1", Name = "Title", Property = "title", Type = ColumnType.Text, Required = false
                },
                new ColumnDto
                {
                    StorageKey = "prop2", Name = "Count", Property = "count", Type = ColumnType.Number, Required = false
                }
            ],
            Statuses =
            [
                new StatusDto { Name = "Open", Color = "green" },
                new StatusDto { Name = "Closed", Color = "gray" }
            ],
            Transitions = []
        };

        _getter.Setup(g => g.GetAsync(_listId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(def);
    }

    private Mock<IGetListItemDefinition> _getter = null!;
    private IMigrationValidator _validator = null!;
    private Guid _listId;

    [Test]
    public async Task RenameStorageKey_Fails_When_SourceMissing_Or_DestinationExists()
    {
        var plan = new MigrationPlan
        {
            RenameStorageKeys =
            [
                new RenameStorageKeyOp("missing", "x"),
                new RenameStorageKeyOp("prop1", "prop2")
            ]
        };

        var result = await _validator.ValidateAsync(_listId, plan, CancellationToken.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSafe, Is.False);
            Assert.That(result.Messages.Any(m => m.Contains("source 'missing' not found")), Is.True);
            Assert.That(result.Messages.Any(m => m.Contains("destination 'prop2'")), Is.True);
        }
    }

    [Test]
    public async Task ChangeColumnType_Fails_Without_Converter()
    {
        var plan = new MigrationPlan
        {
            ChangeColumnTypes = [new ChangeColumnTypeOp("prop1", ColumnType.Number, "")]
        };
        var result = await _validator.ValidateAsync(_listId, plan, CancellationToken.None);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSafe, Is.False);
            Assert.That(result.Messages.Any(m => m.Contains("requires a converter")), Is.True);
        }
    }

    [Test]
    public async Task RemoveStatus_Fails_When_Target_NotFound()
    {
        var plan = new MigrationPlan
        {
            RemoveStatuses = [new RemoveStatusOp("Open", "MissingTarget")]
        };
        var result = await _validator.ValidateAsync(_listId, plan, CancellationToken.None);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSafe, Is.False);
            Assert.That(result.Messages.Any(m => m.Contains("mapping target 'MissingTarget' not found")), Is.True);
        }
    }
}