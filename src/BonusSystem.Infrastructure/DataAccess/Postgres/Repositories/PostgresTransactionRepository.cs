using BonusSystem.Core.Repositories;
using BonusSystem.Shared.Dtos;
using BonusSystem.Shared.Models;

namespace BonusSystem.Infrastructure.DataAccess.Postgres.Repositories;

public class PostgresTransactionRepository : ITransactionRepository
{
    public Task<IEnumerable<TransactionDto>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<TransactionDto?> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Guid> CreateAsync(TransactionDto entity)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateAsync(TransactionDto entity)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<TransactionDto>> GetTransactionsByUserIdAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<TransactionDto>> GetTransactionsByCompanyIdAsync(Guid companyId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<TransactionDto>> GetTransactionsByStoreIdAsync(Guid storeId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<TransactionDto>> GetTransactionsByTypeAsync(TransactionType type)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateTransactionStatusAsync(Guid transactionId, TransactionStatus status)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<TransactionDto>> GetTransactionsInDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        throw new NotImplementedException();
    }

    public Task<decimal> GetTotalBonusCirculationAsync()
    {
        throw new NotImplementedException();
    }

    public Task<decimal> GetTotalActiveBonus()
    {
        throw new NotImplementedException();
    }

    public Task<int> GetTotalTransactionsCountAsync()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<TransactionDto>> GetActiveTransactionsForUserAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<TransactionDto>> GetActiveTransactionsForCompanyAsync(Guid companyId)
    {
        throw new NotImplementedException();
    }

    public Task ProcessExpirationAsync(DateTime expirationDate)
    {
        throw new NotImplementedException();
    }
}