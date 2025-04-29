using BonusSystem.Shared.Models;

namespace BonusSystem.Shared.Dtos;

/// <summary>
/// DTO for store with attached sellers
/// </summary>
public record StoreWithSellersDto
{
    public Guid Id { get; init; }
    public Guid CompanyId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Location { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    public string ContactPhone { get; init; } = string.Empty;
    public StoreStatus Status { get; init; }
    public List<UserDto> Sellers { get; init; } = new();
}

/// <summary>
/// DTO for paginated response of stores with sellers
/// </summary>
public record StoresWithSellersPagedResponseDto
{
    public List<StoreWithSellersDto> Stores { get; init; } = new();
    public int TotalCount { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalPages { get; init; }
}

/// <summary>
/// DTO for filtering stores and sellers
/// </summary>
public record StoresFilterRequestDto
{
    public StoreStatus? StoreStatus { get; init; }
    public UserRole? SellerRole { get; init; } // Always Seller, but included for potential future extensions
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}