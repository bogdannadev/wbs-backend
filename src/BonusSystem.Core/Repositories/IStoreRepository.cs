using BonusSystem.Shared.Dtos;
using BonusSystem.Shared.Models;

namespace BonusSystem.Core.Repositories;

/// <summary>
/// Repository for store-related operations
/// </summary>
public interface IStoreRepository : IRepository<StoreDto, Guid>
{
    Task<IEnumerable<StoreDto>> GetStoresByCompanyIdAsync(Guid companyId);
    Task<bool> UpdateStatusAsync(Guid storeId, StoreStatus status);
    Task<bool> AddSellerToStoreAsync(Guid storeId, Guid sellerId);
    Task<bool> RemoveSellerFromStoreAsync(Guid storeId, Guid sellerId);
    Task<IEnumerable<UserDto>> GetSellersByStoreIdAsync(Guid storeId);
    Task<IEnumerable<StoreDto>> GetStoresByCategoryAsync(string category);
    Task<decimal> GetStoreBonusBalanceAsync(Guid storeId);
}