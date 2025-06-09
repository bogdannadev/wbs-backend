using BonusSystem.Shared.Models;
using BonusSystem.Shared.Validation.Password;
namespace BonusSystem.Shared.Dtos;


public record UserDto
{
    public Guid Id { get; init; }
    public string Username { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public UserRole Role { get; init; }
    public decimal BonusBalance { get; init; }
    public Guid? CompanyId { get; init; }
}

public record BuyerRegistrationDto
{
    public string UserName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    [PasswordValidation]
    public string Password { get; init; } = string.Empty;
}

public record UserRegistrationDto
{
    public string Username { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    [PasswordValidation]
    public string Password { get; init; } = string.Empty;
    public UserRole Role { get; init; }

}

public record UserLoginDto
{
    public string Email { get; init; } = string.Empty;
    [PasswordValidation]
    public string Password { get; init; } = string.Empty;
}

public record UserContextDto
{
    public Guid UserId { get; init; }
    public string Username { get; init; } = string.Empty;
    public UserRole Role { get; init; }
    public decimal BonusBalance { get; init; }
    public Guid? CompanyId { get; init; }
}

public record PermittedActionDto
{
    public string ActionName { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Endpoint { get; init; } = string.Empty;
}