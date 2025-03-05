using BonusSystem.Core.Repositories;
using BonusSystem.Shared.Dtos;
using BonusSystem.Shared.Models;

namespace BonusSystem.Infrastructure.DataAccess.Postgres.Repositories;

public class PostgresStoreRepository : IStoreRepository
{
    public Task<IEnumerable<StoreDto>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<StoreDto?> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Guid> CreateAsync(StoreDto entity)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateAsync(StoreDto entity)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<StoreDto>> GetStoresByCompanyIdAsync(Guid companyId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateStatusAsync(Guid storeId, StoreStatus status)
    {
        throw new NotImplementedException();
    }

    public Task<bool> AddSellerToStoreAsync(Guid storeId, Guid sellerId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> RemoveSellerFromStoreAsync(Guid storeId, Guid sellerId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<UserDto>> GetSellersByStoreIdAsync(Guid storeId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<StoreDto>> GetStoresByCategoryAsync(string category)
    {
        throw new NotImplementedException();
    }

    public Task<decimal> GetStoreBonusBalanceAsync(Guid storeId)
    {
        throw new NotImplementedException();
    }
}