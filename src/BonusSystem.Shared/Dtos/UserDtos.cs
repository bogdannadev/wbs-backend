using BonusSystem.Shared.Models;

namespace BonusSystem.Shared.Dtos;

public record UserDto
{
    public Guid Id { get; init; }
    public string Username { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public UserRole Role { get; init; }
    public decimal BonusBalance { get; init; }
}

public record UserRegistrationDto
{
    public string Username { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public UserRole Role { get; init; }
}

public record UserLoginDto
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}

public record UserContextDto
{
    public Guid UserId { get; init; }
    public string Username { get; init; } = string.Empty;
    public UserRole Role { get; init; }
    public decimal BonusBalance { get; init; }
}

public record PermittedActionDto
{
    public string ActionName { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Endpoint { get; init; } = string.Empty;
}