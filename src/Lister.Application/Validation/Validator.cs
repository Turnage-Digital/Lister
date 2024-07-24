using System.ComponentModel.DataAnnotations;

namespace Lister.Application.Validation;

public class Validator : IValidator
{
    public ValidationResult Validate<T>(T model)
    {
        var retval = new ValidationResult
        {
            IsValid = true
        };

        var properties = typeof(T).GetProperties();
        foreach (var property in properties)
        {
            var attributes = property.GetCustomAttributes(typeof(ValidationAttribute), true);
            foreach (var attribute in attributes)
            {
                if (attribute is ValidationAttribute validationAttribute)
                {
                    var propertyValue = property.CanRead ? property.GetValue(model) : null;
                    var isValid = validationAttribute.IsValid(propertyValue);
                    if (isValid is false)
                    {
                        if (retval.Errors.TryGetValue(property.Name, out var value))
                        {
                            var errors = value.ToList();
                            errors.Add(validationAttribute.FormatErrorMessage(property.Name));
                            retval.Errors[property.Name] = errors.ToArray();
                        }
                        else
                        {
                            retval.Errors.Add(property.Name, [
                                validationAttribute.FormatErrorMessage(property.Name)
                            ]);
                        }

                        retval.IsValid = false;
                    }
                }
            }
        }

        return retval;
    }
}