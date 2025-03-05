using BonusSystem.Core.Repositories;
using BonusSystem.Core.Services.Interfaces;
using BonusSystem.Shared.Dtos;
using BonusSystem.Shared.Models;

namespace BonusSystem.Core.Services.Implementations.BFF;

/// <summary>
/// BFF service for Buyer role
/// </summary>
public class BuyerBffService : BaseBffService, IBuyerBffService
{
    public BuyerBffService(
        IDataService dataService,
        IAuthenticationService authService) 
        : base(dataService, authService)
    {
    }

    /// <summary>
    /// Gets the permitted actions for a buyer
    /// </summary>
    public override async Task<IEnumerable<PermittedActionDto>> GetPermittedActionsAsync(Guid userId)
    {
        var role = await _dataService.Users.GetUserRoleAsync(userId);
        if (role != UserRole.Buyer)
        {
            return Enumerable.Empty<PermittedActionDto>();
        }

        return new List<PermittedActionDto>
        {
            new() { ActionName = "ViewBalance", Description = "View bonus balance", Endpoint = "/api/buyers/balance" },
            new() { ActionName = "ViewTransactions", Description = "View transaction history", Endpoint = "/api/buyers/transactions" },
            new() { ActionName = "GenerateQrCode", Description = "Generate QR code", Endpoint = "/api/buyers/qrcode" },
            new() { ActionName = "CancelTransaction", Description = "Cancel transaction", Endpoint = "/api/buyers/transactions/{id}/cancel" },
            new() { ActionName = "FindStores", Description = "Find stores by category", Endpoint = "/api/buyers/stores" }
        };
    }

    /// <summary>
    /// Gets the bonus summary for a buyer
    /// </summary>
    public async Task<BonusTransactionSummaryDto> GetBonusSummaryAsync(Guid userId)
    {
        var transactions = await _dataService.Transactions.GetTransactionsByUserIdAsync(userId);

        // Calculate earned and spent amounts
        var earned = transactions
            .Where(t => t.Type == TransactionType.Earn && t.Status == TransactionStatus.Completed)
            .Sum(t => t.Amount);

        var spent = transactions
            .Where(t => t.Type == TransactionType.Spend && t.Status == TransactionStatus.Completed)
            .Sum(t => t.Amount);

        // Get current user balance
        var user = await _dataService.Users.GetByIdAsync(userId);
        var currentBalance = user?.BonusBalance ?? 0m;

        // Get expiring amount - for prototype, this is just a placeholder
        // In a real implementation, this would consider the quarterly expiration rules
        var expiringNextQuarter = currentBalance * 0.5m;

        // Get recent transactions
        var recentTransactions = transactions
            .OrderByDescending(t => t.Timestamp)
            .Take(5)
            .ToList();

        return new BonusTransactionSummaryDto
        {
            TotalEarned = earned,
            TotalSpent = spent,
            CurrentBalance = currentBalance,
            ExpiringNextQuarter = expiringNextQuarter,
            RecentTransactions = recentTransactions
        };
    }

    /// <summary>
    /// Gets the transaction history for a buyer
    /// </summary>
    public async Task<IEnumerable<TransactionDto>> GetTransactionHistoryAsync(Guid userId)
    {
        var transactions = await _dataService.Transactions.GetTransactionsByUserIdAsync(userId);
        return transactions.OrderByDescending(t => t.Timestamp);
    }

    /// <summary>
    /// Generates a QR code for a buyer
    /// </summary>
    public Task<string> GenerateQrCodeAsync(Guid userId)
    {
        // In a real implementation, this would generate an actual QR code
        // For the prototype, we'll just return a placeholder
        var qrData = $"BONUS-USER-{userId:N}";
        return Task.FromResult(qrData);
    }

    /// <summary>
    /// Cancels a transaction
    /// </summary>
    public async Task<bool> CancelTransactionAsync(Guid userId, Guid transactionId)
    {
        var transaction = await _dataService.Transactions.GetByIdAsync(transactionId);
        
        // Check if the transaction exists and belongs to the user
        if (transaction == null || transaction.UserId != userId)
        {
            return false;
        }

        // Check if the transaction can be cancelled (i.e., is recent enough)
        if (transaction.Timestamp < DateTime.UtcNow.AddDays(-1))
        {
            // Transactions can only be cancelled within 24 hours (example rule)
            return false;
        }

        // Update transaction status
        await _dataService.Transactions.UpdateTransactionStatusAsync(transactionId, TransactionStatus.Reversed);

        // Adjust user balance accordingly
        var user = await _dataService.Users.GetByIdAsync(userId);
        if (user == null)
        {
            return false;
        }

        decimal newBalance = user.BonusBalance;
        if (transaction.Type == TransactionType.Earn)
        {
            // If it was an earn transaction, subtract the amount
            newBalance -= transaction.Amount;
        }
        else if (transaction.Type == TransactionType.Spend)
        {
            // If it was a spend transaction, add the amount back
            newBalance += transaction.Amount;
        }

        await _dataService.Users.UpdateBalanceAsync(userId, newBalance);

        return true;
    }

    /// <summary>
    /// Finds stores by category
    /// </summary>
    public async Task<IEnumerable<StoreDto>> FindStoresByCategoryAsync(string category)
    {
        return await _dataService.Stores.GetStoresByCategoryAsync(category);
    }
}
