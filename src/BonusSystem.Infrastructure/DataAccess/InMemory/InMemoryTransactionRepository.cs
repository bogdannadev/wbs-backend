using BonusSystem.Core.Repositories;
using BonusSystem.Shared.Dtos;
using BonusSystem.Shared.Models;

namespace BonusSystem.Infrastructure.DataAccess.InMemory;

/// <summary>
/// In-memory implementation of the transaction repository
/// </summary>
public class InMemoryTransactionRepository : InMemoryRepository<TransactionDto, Guid>, ITransactionRepository
{
    public InMemoryTransactionRepository() : base(transaction => transaction.Id)
    {
        // Initialize with some sample data
        var transactions = new List<TransactionDto>
        {
            new()
            {
                Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                UserId = Guid.Parse("11111111-1111-1111-1111-111111111111"), // buyer1
                CompanyId = Guid.Parse("44444444-4444-4444-4444-444444444444"), // Alpha Store Chain
                StoreId = Guid.Parse("77777777-7777-7777-7777-777777777777"), // Alpha Downtown
                Amount = 100,
                Type = TransactionType.Earn,
                Timestamp = DateTime.UtcNow.AddDays(-10),
                Status = TransactionStatus.Completed,
                Description = "Purchase at Alpha Downtown Store"
            },
            new()
            {
                Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                UserId = Guid.Parse("11111111-1111-1111-1111-111111111111"), // buyer1
                CompanyId = Guid.Parse("44444444-4444-4444-4444-444444444444"), // Alpha Store Chain
                StoreId = Guid.Parse("77777777-7777-7777-7777-777777777777"), // Alpha Downtown
                Amount = 50,
                Type = TransactionType.Spend,
                Timestamp = DateTime.UtcNow.AddDays(-5),
                Status = TransactionStatus.Completed,
                Description = "Redemption at Alpha Downtown Store"
            },
            new()
            {
                Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                UserId = Guid.Parse("11111111-1111-1111-1111-111111111111"), // buyer1
                CompanyId = Guid.Parse("55555555-5555-5555-5555-555555555555"), // Beta Retail Group
                StoreId = Guid.Parse("99999999-9999-9999-9999-999999999999"), // Beta Central
                Amount = 75,
                Type = TransactionType.Earn,
                Timestamp = DateTime.UtcNow.AddDays(-2),
                Status = TransactionStatus.Completed,
                Description = "Purchase at Beta Central Store"
            }
        };

        foreach (var transaction in transactions)
        {
            _entities[transaction.Id] = transaction;
        }
    }

    public Task<IEnumerable<TransactionDto>> GetTransactionsByUserIdAsync(Guid userId)
    {
        var transactions = _entities.Values.Where(t => t.UserId == userId);
        return Task.FromResult(transactions);
    }

    public Task<IEnumerable<TransactionDto>> GetTransactionsByCompanyIdAsync(Guid companyId)
    {
        var transactions = _entities.Values.Where(t => t.CompanyId == companyId);
        return Task.FromResult(transactions);
    }

    public Task<IEnumerable<TransactionDto>> GetTransactionsByStoreIdAsync(Guid storeId)
    {
        var transactions = _entities.Values.Where(t => t.StoreId == storeId);
        return Task.FromResult(transactions);
    }

    public Task<IEnumerable<TransactionDto>> GetTransactionsByTypeAsync(TransactionType type)
    {
        var transactions = _entities.Values.Where(t => t.Type == type);
        return Task.FromResult(transactions);
    }

    public Task<bool> UpdateTransactionStatusAsync(Guid transactionId, TransactionStatus status)
    {
        if (!_entities.TryGetValue(transactionId, out var transaction))
        {
            return Task.FromResult(false);
        }

        var updatedTransaction = transaction with { Status = status };
        return Task.FromResult(_entities.TryUpdate(transactionId, updatedTransaction, transaction));
    }

    public Task<IEnumerable<TransactionDto>> GetTransactionsInDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        var transactions = _entities.Values.Where(t => 
            t.Timestamp >= startDate && t.Timestamp <= endDate);
        
        return Task.FromResult(transactions);
    }

    public Task<decimal> GetTotalBonusCirculationAsync()
    {
        // Sum of all earned bonuses minus spent bonuses
        var earned = _entities.Values
            .Where(t => t.Type == TransactionType.Earn && t.Status == TransactionStatus.Completed)
            .Sum(t => t.Amount);
        
        var spent = _entities.Values
            .Where(t => t.Type == TransactionType.Spend && t.Status == TransactionStatus.Completed)
            .Sum(t => t.Amount);
        
        return Task.FromResult(earned - spent);
    }

    public Task<decimal> GetTotalActiveBonus()
    {
        return GetTotalBonusCirculationAsync();
    }

    public Task<int> GetTotalTransactionsCountAsync()
    {
        return Task.FromResult(_entities.Count);
    }

    public Task<IEnumerable<TransactionDto>> GetActiveTransactionsForUserAsync(Guid userId)
    {
        var transactions = _entities.Values.Where(t => 
            t.UserId == userId && 
            t.Status == TransactionStatus.Completed &&
            (t.Type == TransactionType.Earn || t.Type == TransactionType.AdminAdjustment));
        
        return Task.FromResult(transactions);
    }

    public Task<IEnumerable<TransactionDto>> GetActiveTransactionsForCompanyAsync(Guid companyId)
    {
        var transactions = _entities.Values.Where(t => 
            t.CompanyId == companyId && 
            t.Status == TransactionStatus.Completed);
        
        return Task.FromResult(transactions);
    }

    public Task ProcessExpirationAsync(DateTime expirationDate)
    {
        // Find all active transactions that need to expire
        var transactionsToExpire = _entities.Values
            .Where(t => 
                t.Status == TransactionStatus.Completed && 
                t.Type == TransactionType.Earn &&
                t.Timestamp < expirationDate)
            .ToList();
        
        // Create expiration transactions for each
        foreach (var transaction in transactionsToExpire)
        {
            var expirationTransaction = new TransactionDto
            {
                Id = Guid.NewGuid(),
                UserId = transaction.UserId,
                CompanyId = transaction.CompanyId,
                Amount = transaction.Amount,
                Type = TransactionType.Expire,
                Timestamp = DateTime.UtcNow,
                Status = TransactionStatus.Completed,
                Description = $"Expiration of transaction {transaction.Id}"
            };
            
            _entities[expirationTransaction.Id] = expirationTransaction;
        }
        
        return Task.CompletedTask;
    }
}