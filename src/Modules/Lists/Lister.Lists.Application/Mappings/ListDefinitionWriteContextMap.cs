using System.Linq;
using Lister.Lists.Domain;
using Lister.Lists.Domain.ValueObjects;
using Lister.Lists.ReadOnly.Dtos;

namespace Lister.Lists.Application.Mappings;

internal static class ListDefinitionWriteContextMap
{
    public static ListItemDefinitionDto ToDto(
        IList list,
        Column[] columns,
        Status[] statuses,
        StatusTransition[] transitions
    ) =>
        new()
        {
            Id = list.Id,
            Name = list.Name,
            Columns = columns.Select(ToDto).ToArray(),
            Statuses = statuses.Select(ToDto).ToArray(),
            Transitions = transitions.Select(ToDto).ToArray()
        };

    private static ColumnDto ToDto(Column column) =>
        new()
        {
            StorageKey = column.StorageKey,
            Name = column.Name,
            Property = column.Property,
            Type = column.Type,
            Required = column.Required,
            AllowedValues = column.AllowedValues,
            MinNumber = column.MinNumber,
            MaxNumber = column.MaxNumber,
            Regex = column.Regex
        };

    private static StatusDto ToDto(Status status) =>
        new()
        {
            Name = status.Name,
            Color = status.Color
        };

    private static StatusTransitionDto ToDto(StatusTransition transition) =>
        new()
        {
            From = transition.From,
            AllowedNext = transition.AllowedNext
        };
}
