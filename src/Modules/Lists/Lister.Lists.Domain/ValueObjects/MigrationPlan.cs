using System.Text.Json.Serialization;
using Lister.Lists.Domain.Enums;

namespace Lister.Lists.Domain.ValueObjects;

public class MigrationPlan
{
    [JsonPropertyName("changeColumnTypes")]
    public ChangeColumnTypeOp[]? ChangeColumnTypes { get; init; }

    [JsonPropertyName("removeColumns")]
    public RemoveColumnOp[]? RemoveColumns { get; init; }

    [JsonPropertyName("tightenConstraints")]
    public TightenConstraintsOp[]? TightenConstraints { get; init; }

    [JsonPropertyName("renameStorageKeys")]
    public RenameStorageKeyOp[]? RenameStorageKeys { get; init; }

    [JsonPropertyName("removeStatuses")]
    public RemoveStatusOp[]? RemoveStatuses { get; init; }
}

public record ChangeColumnTypeOp(
    string Key,
    ColumnType TargetType,
    string Converter
);

public record RemoveColumnOp(
    string Key,
    string Policy
);

public record TightenConstraintsOp(
    string Key,
    bool? Required,
    string[]? AllowedValues,
    decimal? MinNumber,
    decimal? MaxNumber,
    string? Regex
);

public record RenameStorageKeyOp(
    string From,
    string To
);

public record RemoveStatusOp(
    string Name,
    string? MapTo
);