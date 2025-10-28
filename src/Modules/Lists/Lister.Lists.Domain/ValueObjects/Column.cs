using Lister.Lists.Domain.Enums;

namespace Lister.Lists.Domain.ValueObjects;

public record Column
{
    public string? StorageKey { get; set; }

    public string Name { get; set; } = null!;

    public string Property
    {
        get
        {
            var nameWithoutSpaces = Name.Replace(" ", string.Empty);
            var nameWithoutSpecialChars = new string(nameWithoutSpaces.Where(char.IsLetterOrDigit).ToArray());
            var retval = char.ToLowerInvariant(nameWithoutSpecialChars[0]) + nameWithoutSpecialChars[1..];
            return retval;
        }
    }

    public ColumnType Type { get; set; }

    public bool Required { get; set; }

    public string[]? AllowedValues { get; set; }

    public decimal? MinNumber { get; set; }

    public decimal? MaxNumber { get; set; }

    public string? Regex { get; set; }
}