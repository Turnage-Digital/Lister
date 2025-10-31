namespace Lister.Lists.Domain.ValueObjects;

public record Status
{
    public string Name { get; set; } = null!;

    public string Color { get; set; } = null!;
}