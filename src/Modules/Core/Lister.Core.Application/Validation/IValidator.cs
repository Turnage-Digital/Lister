namespace Lister.Core.Application.Validation;

public interface IValidator
{
    ValidationResult Validate<T>(T model);
}