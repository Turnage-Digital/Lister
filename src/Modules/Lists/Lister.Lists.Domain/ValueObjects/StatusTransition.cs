namespace Lister.Lists.Domain.ValueObjects;

public record StatusTransition
{
    public string From { get; init; } = string.Empty;

    public string[] AllowedNext { get; init; } = [];
}