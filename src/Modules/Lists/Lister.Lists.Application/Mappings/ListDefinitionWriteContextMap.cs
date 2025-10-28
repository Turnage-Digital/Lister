using Lister.Lists.Domain.Entities;
using Lister.Lists.Domain.ValueObjects;
using Lister.Lists.ReadOnly.Dtos;

namespace Lister.Lists.Application.Mappings;

internal static class ListDefinitionWriteContextMap
{
    public static ListItemDefinitionDto ToDto(
        IWritableList list,
        Column[] columns,
        Status[] statuses,
        StatusTransition[] transitions
    )
    {
        return new ListItemDefinitionDto
        {
            Id = list.Id,
            Name = list.Name,
            Columns = columns.Select(ToDto).ToArray(),
            Statuses = statuses.Select(ToDto).ToArray(),
            Transitions = transitions.Select(ToDto).ToArray()
        };
    }

    private static ColumnDto ToDto(Column column)
    {
        return new ColumnDto
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
    }

    private static StatusDto ToDto(Status status)
    {
        return new StatusDto
        {
            Name = status.Name,
            Color = status.Color
        };
    }

    private static StatusTransitionDto ToDto(StatusTransition transition)
    {
        return new StatusTransitionDto
        {
            From = transition.From,
            AllowedNext = transition.AllowedNext
        };
    }
}