using BonusSystem.Shared.Models;

namespace BonusSystem.Shared.Dtos;

public record CompanyDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string ContactEmail { get; init; } = string.Empty;
    public string ContactPhone { get; init; } = string.Empty;
    public decimal BonusBalance { get; init; }
    public decimal OriginalBonusBalance { get; init; }
    public CompanyStatus Status { get; init; }
    public DateTime CreatedAt { get; init; }
    public List<StoreDto> Stores { get; init; } = new();
}

public record CompanyRegistrationDto
{
    public string Name { get; init; } = string.Empty;
    public string ContactEmail { get; init; } = string.Empty;
    public string ContactPhone { get; init; } = string.Empty;
    public decimal InitialBonusBalance { get; init; }
}

public record CompanyRegistrationResultDto
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
    public CompanyDto? Company { get; init; }
}

public record StoreDto
{
    public Guid Id { get; init; }
    public Guid CompanyId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Location { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    public string ContactPhone { get; init; } = string.Empty;
    public StoreStatus Status { get; init; }
}

public record StoreRegistrationDto
{
    public Guid CompanyId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Location { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    public string ContactPhone { get; init; } = string.Empty;
    public List<Guid> SellerIds { get; init; } = new();
}

public record CompanySummaryDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal BonusBalance { get; init; }
    public decimal TransactionVolume { get; init; }
    public int StoreCount { get; init; }
    public CompanyStatus Status { get; init; }
}