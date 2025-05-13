using BonusSystem.Infrastructure.DataAccess.Entities;
using BonusSystem.Shared.Models; 

namespace BonusSystem.Core.Repositories;

public interface IFiatTransactionRepository
{
    Task<IEnumerable<FiatTransactionEntity>> GetAllAsync();
    Task<FiatTransactionEntity?> GetByIdAsync(Guid id);
    Task<Guid> CreateAsync(FiatTransactionEntity entity);
    Task<bool> UpdateAsync(FiatTransactionEntity entity);
    Task<bool> DeleteAsync(Guid id);
}