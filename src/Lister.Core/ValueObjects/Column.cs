using System.Text.Json.Serialization;
using Lister.Core.Enums;

namespace Lister.Core.ValueObjects;

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
}