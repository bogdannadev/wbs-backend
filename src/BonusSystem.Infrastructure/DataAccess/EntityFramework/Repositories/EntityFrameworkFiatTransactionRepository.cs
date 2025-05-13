using BonusSystem.Core.Repositories;
using BonusSystem.Infrastructure.DataAccess.Entities;
using BonusSystem.Shared.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BonusSystem.Infrastructure.DataAccess.EntityFramework.Repositories;

public class EntityFrameworkFiatTransactionRepository : IFiatTransactionRepository
{
    private readonly BonusSystemContext _dbContext;
    private readonly ILogger<EntityFrameworkFiatTransactionRepository> _logger;

    public EntityFrameworkFiatTransactionRepository(BonusSystemContext dbContext, ILogger<EntityFrameworkFiatTransactionRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<IEnumerable<FiatTransactionEntity>> GetAllAsync()
    {
        try
        {
            return await _dbContext.FiatTransactions.AsNoTracking().ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all fiat transactions");
            throw;
        }
    }

    public async Task<FiatTransactionEntity?> GetByIdAsync(Guid id)
    {
        try
        {
            return await _dbContext.FiatTransactions.AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving fiat transaction with ID {Id}", id);
            throw;
        }
    }

    public async Task<Guid> CreateAsync(FiatTransactionEntity entity)
    {
        try
        {
            entity.TransactionDate = DateTime.UtcNow;
            await _dbContext.FiatTransactions.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating fiat transaction");
            throw;
        }
    }

    public async Task<bool> UpdateAsync(FiatTransactionEntity entity)
    {
        try
        {
            _dbContext.FiatTransactions.Update(entity);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating fiat transaction with ID {Id}", entity.Id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        try
        {
            var entity = await _dbContext.FiatTransactions.FindAsync(id);
            if (entity == null)
            {
                return false;
            }

            _dbContext.FiatTransactions.Remove(entity);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting fiat transaction with ID {Id}", id);
            throw;
        }
    }
}