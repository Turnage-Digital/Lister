using Lister.Core.Enums;
using Newtonsoft.Json;

namespace Lister.Core.ValueObjects;

public record Column
{
    [JsonProperty("name")]
    public string Name { get; set; } = null!;

    [JsonProperty("property")]
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

    [JsonProperty("type")]
    public ColumnType Type { get; set; }
}