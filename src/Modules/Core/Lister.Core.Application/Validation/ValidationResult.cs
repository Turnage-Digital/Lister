namespace Lister.Core.Application.Validation;

public class ValidationResult
{
    public bool IsValid { get; set; }

    public bool IsNotValid => IsValid is false;

    public Dictionary<string, string[]> Errors { get; set; } = new();
}