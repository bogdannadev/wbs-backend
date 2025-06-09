using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace BonusSystem.Shared.Validation.Password; 
public class PasswordValidationAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var password = value as string ?? "";

        if (password.Length < 8)
            return new ValidationResult("Password must be at least 8 characters long.");

        if (!Regex.IsMatch(password, @"[A-Z]"))
            return new ValidationResult("Password must contain an uppercase letter.");

        if (!Regex.IsMatch(password, @"[a-z]"))
            return new ValidationResult("Password must contain a lowercase letter.");

        if (!Regex.IsMatch(password, @"\d"))
            return new ValidationResult("Password must contain a digit.");

        if (!Regex.IsMatch(password, @"[\W_]"))
            return new ValidationResult("Password must contain a special character.");

        return ValidationResult.Success;
    }
}
