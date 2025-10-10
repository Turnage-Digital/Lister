using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Lister.Lists.Domain.Entities;
using Lister.Lists.Domain.Enums;

namespace Lister.Lists.Domain;

public class ListItemBagValidator<TList, TItem>(IListsUnitOfWork<TList, TItem> unitOfWork)
    : IValidateListItemBag<TList>
    where TList : IWritableList
    where TItem : IWritableItem
{
    public async Task ValidateAsync(TList list, object bag, CancellationToken cancellationToken)
    {
        var columns = await unitOfWork.ListsStore.GetColumnsAsync(list, cancellationToken);
        var dict = bag as IDictionary<string, object?>;

        foreach (var col in columns)
        {
            var key = col.Property;
            if (dict is not null && dict.TryGetValue(key, out var value))
            {
                if (!IsTypeCompatible(col.Type, value))
                {
                    throw new InvalidOperationException($"Bag property '{key}' is not of expected type {col.Type}");
                }

                if (col.Required && col.Type == ColumnType.Text && value is string s && string.IsNullOrWhiteSpace(s))
                {
                    throw new InvalidOperationException($"Bag property '{key}' cannot be empty");
                }

                if (col.AllowedValues is { Length: > 0 })
                {
                    var valStr = value?.ToString();
                    if (valStr is not null && !col.AllowedValues.Contains(valStr, StringComparer.OrdinalIgnoreCase))
                    {
                        throw new InvalidOperationException(
                            $"Bag property '{key}' value '{valStr}' is not in allowed values");
                    }
                }

                if (col.Type == ColumnType.Number && value is not null &&
                    (col.MinNumber is not null || col.MaxNumber is not null))
                {
                    try
                    {
                        var num = Convert.ToDecimal(value);
                        if (col.MinNumber is not null && num < col.MinNumber)
                        {
                            throw new InvalidOperationException(
                                $"Bag property '{key}' value '{num}' is less than minimum {col.MinNumber}");
                        }

                        if (col.MaxNumber is not null && num > col.MaxNumber)
                        {
                            throw new InvalidOperationException(
                                $"Bag property '{key}' value '{num}' exceeds maximum {col.MaxNumber}");
                        }
                    }
                    catch (FormatException)
                    {
                        throw new InvalidOperationException($"Bag property '{key}' is not a valid number");
                    }
                }

                if (col.Type == ColumnType.Text && value is string sv && !string.IsNullOrWhiteSpace(col.Regex))
                {
                    if (!Regex.IsMatch(sv, col.Regex!))
                    {
                        throw new InvalidOperationException(
                            $"Bag property '{key}' value does not match required pattern");
                    }
                }
            }
            else if (dict is not null && col.Required)
            {
                throw new InvalidOperationException($"Bag property '{key}' is required");
            }
        }

        if (dict is not null && dict.TryGetValue("status", out var statusVal) && statusVal is string statusStr)
        {
            var statuses = await unitOfWork.ListsStore.GetStatusesAsync(list, cancellationToken);
            if (!statuses.Any(s => string.Equals(s.Name, statusStr, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException($"Status '{statusStr}' is not valid for this list");
            }
        }
    }

    private static bool IsTypeCompatible(ColumnType type, object? value)
    {
        if (value is null)
        {
            return true;
        }

        return type switch
        {
            ColumnType.Text => value is string,
            ColumnType.Number => value is sbyte or byte or short or ushort or int or uint or long or ulong or float
                or double or decimal,
            ColumnType.Boolean => value is bool,
            ColumnType.Date => value is DateTime or string,
            _ => true
        };
    }
}
