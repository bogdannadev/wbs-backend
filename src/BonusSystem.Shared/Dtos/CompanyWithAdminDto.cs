using BonusSystem.Shared.Models;

namespace BonusSystem.Shared.Dtos;

/// <summary>
/// DTO for registering a company with its admin user
/// </summary>
public record CompanyWithAdminRegistrationDto
{
    // Company Information
    public string Name { get; init; } = string.Empty;
    public string ContactEmail { get; init; } = string.Empty;
    public string ContactPhone { get; init; } = string.Empty;
    public decimal InitialBonusBalance { get; init; }
    
    // Company Admin Information
    public string AdminUsername { get; init; } = string.Empty;
    public string AdminEmail { get; init; } = string.Empty;
    public string AdminPassword { get; init; } = string.Empty;
}

/// <summary>
/// DTO for registering a seller with explicit company ID
/// </summary>
public record SellerRegistrationDto
{
    public string Username { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}
