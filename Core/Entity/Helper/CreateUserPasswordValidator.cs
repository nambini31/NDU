using System.ComponentModel.DataAnnotations;

namespace Core.Entity.Helper;

public class CreateUserPasswordValidator : ValidationAttribute
{
    private readonly int _minLength;

    public CreateUserPasswordValidator(int minLength)
    {
        _minLength = minLength;
    }

    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        var currentValue = $"{value}";
        var attributeName = validationContext.DisplayName;
        var updatePasswordValue = validationContext.ObjectType.GetProperty("UpdatePassword")?.GetValue(validationContext.ObjectInstance)?.ToString();
        var identifiant = validationContext.ObjectType.GetProperty("Identifiant")?.GetValue(validationContext.ObjectInstance)?.ToString();

        if (identifiant == currentValue) { return new ValidationResult($"Username must not be the same as the password."); };
        if (currentValue.Length < _minLength) { return new ValidationResult($"The field {attributeName} must have at least {_minLength} characters."); }
        if (bool.TryParse(updatePasswordValue, out var updatePassword) && !updatePassword) { return ValidationResult.Success!; }
        return  ValidationResult.Success!;
    }
}