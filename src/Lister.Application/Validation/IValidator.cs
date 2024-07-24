namespace Lister.Application.Validation;

public interface IValidator
{
    ValidationResult Validate<T>(T model);
}