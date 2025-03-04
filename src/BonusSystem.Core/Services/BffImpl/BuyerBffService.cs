using BonusSystem.Core.Repositories;
using BonusSystem.Core.Services.Interfaces;
using BonusSystem.Shared.Dtos;
using BonusSystem.Shared.Models;
using Microsoft.Extensions.Logging;
using System.Text;

namespace BonusSystem.Core.Services.BffImpl;

/// <summary>
/// Implementation of BFF service for Buyer users
/// </summary>
public class BuyerBffService : BaseBffService, IBuyerBffService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IStoreRepository _storeRepository;

    public BuyerBffService(
        IUserRepository userRepository,
        ITransactionRepository transactionRepository,
        IStoreRepository storeRepository,
        ILogger<BuyerBffService> logger) 
        : base(userRepository, logger)
    {
        _transactionRepository = transactionRepository;
        _storeRepository = storeRepository;
    }

    /// <summary>
    /// Get buyer-specific permitted actions
    /// </summary>
    public override async Task<IEnumerable<PermittedActionDto>> GetPermittedActionsAsync(Guid userId)
    {
        await ValidateUserRoleAsync(userId, UserRole.Buyer);

        return new List<PermittedActionDto>
        {
            new() { ActionName = "ViewBonusSummary", Description = "View bonus summary", Endpoint = "/api/buyer/bonus-summary" },
            new() { ActionName = "ViewTransactionHistory", Description = "View transaction history", Endpoint = "/api/buyer/transactions" },
            new() { ActionName = "GenerateQrCode", Description = "Generate QR code for purchase", Endpoint = "/api/buyer/qr-code" },
            new() { ActionName = "CancelTransaction", Description = "Cancel a transaction", Endpoint = "/api/buyer/transactions/{id}/cancel" },
            new() { ActionName = "FindStoresByCategory", Description = "Find stores by category", Endpoint = "/api/buyer/stores" }
        };
    }

    /// <summary>
    /// Get bonus summary for a buyer
    /// </summary>
    public async Task<BonusTransactionSummaryDto> GetBonusSummaryAsync(Guid userId)
    {
        await ValidateUserRoleAsync(userId, UserRole.Buyer);

        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {userId} not found");
            }

            var transactions = await _transactionRepository.GetTransactionsByUserIdAsync(userId);
            
            var totalEarned = transactions
                .Where(t => t.Type == TransactionType.Earn && t.Status == TransactionStatus.Completed)
                .Sum(t => t.Amount);
            
            var totalSpent = transactions
                .Where(t => t.Type == TransactionType.Spend && t.Status == TransactionStatus.Completed)
                .Sum(t => t.Amount);

            // For the prototype, we'll simulate expiring bonus calculation
            // In a real implementation, this would involve more complex logic based on transaction dates
            var expiringNextQuarter = Math.Min(user.BonusBalance * 0.25m, user.BonusBalance);

            return new BonusTransactionSummaryDto
            {
                TotalEarned = totalEarned,
                TotalSpent = totalSpent,
                CurrentBalance = user.BonusBalance,
                ExpiringNextQuarter = expiringNextQuarter,
                RecentTransactions = transactions
                    .Where(t => t.Status == TransactionStatus.Completed)
                    .OrderByDescending(t => t.Timestamp)
                    .Take(5)
                    .ToList()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting bonus summary for user {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Get transaction history for a buyer
    /// </summary>
    public async Task<IEnumerable<TransactionDto>> GetTransactionHistoryAsync(Guid userId)
    {
        await ValidateUserRoleAsync(userId, UserRole.Buyer);

        try
        {
            var transactions = await _transactionRepository.GetTransactionsByUserIdAsync(userId);
            return transactions.OrderByDescending(t => t.Timestamp);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting transaction history for user {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Generate QR code for a buyer
    /// </summary>
    public async Task<string> GenerateQrCodeAsync(Guid userId)
    {
        await ValidateUserRoleAsync(userId, UserRole.Buyer);

        try
        {
            // For the prototype, we'll generate a simple representation of a QR code
            // In a real implementation, this would use a QR code generation library
            
            // Format: userid-timestamp-randomsuffix
            var qrContent = $"{userId}-{DateTime.UtcNow.Ticks}-{Guid.NewGuid().ToString()[..8]}";
            
            // For the prototype, we'll return the QR content as a Base64 string
            // In a real implementation, this would be an actual QR code image
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(qrContent));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating QR code for user {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Cancel a transaction
    /// </summary>
    public async Task<bool> CancelTransactionAsync(Guid userId, Guid transactionId)
    {
        await ValidateUserRoleAsync(userId, UserRole.Buyer);

        try
        {
            var transaction = await _transactionRepository.GetByIdAsync(transactionId);
            if (transaction == null)
            {
                throw new KeyNotFoundException($"Transaction with ID {transactionId} not found");
            }

            if (transaction.UserId != userId)
            {
                throw new UnauthorizedAccessException("You do not have permission to cancel this transaction");
            }

            if (transaction.Status != TransactionStatus.Completed)
            {
                throw new InvalidOperationException($"Transaction with status {transaction.Status} cannot be cancelled");
            }

            // Mark the transaction as cancelled
            await _transactionRepository.UpdateTransactionStatusAsync(transactionId, TransactionStatus.Reversed);

            // Update the user's balance
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {userId} not found");
            }

            decimal newBalance;
            if (transaction.Type == TransactionType.Earn)
            {
                // If earned, remove the bonus
                newBalance = user.BonusBalance - transaction.Amount;
            }
            else
            {
                // If spent, add back the bonus
                newBalance = user.BonusBalance + transaction.Amount;
            }

            await _userRepository.UpdateBalanceAsync(userId, newBalance);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling transaction {TransactionId} for user {UserId}", transactionId, userId);
            throw;
        }
    }

    /// <summary>
    /// Find stores by category
    /// </summary>
    public async Task<IEnumerable<StoreDto>> FindStoresByCategoryAsync(string category)
    {
        try
        {
            // For the prototype, we'll just fetch all stores
            // In a real implementation, this would filter by category
            var stores = await _storeRepository.GetStoresByCategoryAsync(category);
            return stores;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finding stores by category {Category}", category);
            throw;
        }
    }
}