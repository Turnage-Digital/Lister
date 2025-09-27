using System.Text.Json.Serialization;
using Lister.Lists.Domain.Enums;

namespace Lister.Lists.Domain.ValueObjects;

public record Column
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("property")]
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

    [JsonPropertyName("type")]
    public ColumnType Type { get; set; }

    [JsonPropertyName("required")]
    public bool Required { get; set; }

    [JsonPropertyName("allowedValues")]
    public string[]? AllowedValues { get; set; }

    [JsonPropertyName("minNumber")]
    public decimal? MinNumber { get; set; }

    [JsonPropertyName("maxNumber")]
    public decimal? MaxNumber { get; set; }

    [JsonPropertyName("regex")]
    public string? Regex { get; set; }
}