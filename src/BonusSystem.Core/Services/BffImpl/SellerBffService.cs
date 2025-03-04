using BonusSystem.Core.Repositories;
using BonusSystem.Core.Services.Interfaces;
using BonusSystem.Shared.Dtos;
using BonusSystem.Shared.Models;
using Microsoft.Extensions.Logging;

namespace BonusSystem.Core.Services.BffImpl;

/// <summary>
/// Implementation of BFF service for Seller users
/// </summary>
public class SellerBffService : BaseBffService, ISellerBffService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IStoreRepository _storeRepository;
    private readonly ICompanyRepository _companyRepository;

    public SellerBffService(
        IUserRepository userRepository,
        ITransactionRepository transactionRepository,
        IStoreRepository storeRepository,
        ICompanyRepository companyRepository,
        ILogger<SellerBffService> logger) 
        : base(userRepository, logger)
    {
        _transactionRepository = transactionRepository;
        _storeRepository = storeRepository;
        _companyRepository = companyRepository;
    }

    /// <summary>
    /// Get seller-specific permitted actions
    /// </summary>
    public override async Task<IEnumerable<PermittedActionDto>> GetPermittedActionsAsync(Guid userId)
    {
        await ValidateUserRoleAsync(userId, UserRole.Seller);

        return new List<PermittedActionDto>
        {
            new() { ActionName = "ProcessTransaction", Description = "Process a bonus transaction", Endpoint = "/api/seller/transactions" },
            new() { ActionName = "ConfirmTransactionReturn", Description = "Confirm a transaction return", Endpoint = "/api/seller/transactions/{id}/return" },
            new() { ActionName = "GetBuyerBonusBalance", Description = "Get a buyer's bonus balance", Endpoint = "/api/seller/buyers/{id}/balance" },
            new() { ActionName = "GetStoreBonusBalance", Description = "Get a store's bonus balance", Endpoint = "/api/seller/stores/{id}/balance" },
            new() { ActionName = "GetStoreBonusTransactions", Description = "Get store bonus transactions", Endpoint = "/api/seller/stores/{id}/transactions" }
        };
    }

    /// <summary>
    /// Process a transaction (bonus earn or spend)
    /// </summary>
    public async Task<TransactionResultDto> ProcessTransactionAsync(Guid sellerId, TransactionRequestDto request)
    {
        await ValidateUserRoleAsync(sellerId, UserRole.Seller);

        try
        {
            // Verify the buyer exists
            var buyer = await _userRepository.GetByIdAsync(request.BuyerId);
            if (buyer == null)
            {
                return new TransactionResultDto
                {
                    Success = false,
                    ErrorMessage = $"Buyer with ID {request.BuyerId} not found"
                };
            }

            // Verify the store exists and seller is associated with it
            // In a real implementation, we would verify the seller is associated with the store
            var store = await _storeRepository.GetByIdAsync(request.StoreId);
            if (store == null)
            {
                return new TransactionResultDto
                {
                    Success = false,
                    ErrorMessage = $"Store with ID {request.StoreId} not found"
                };
            }

            // If it's a spend transaction, check if the buyer has enough bonus balance
            if (request.Type == TransactionType.Spend && buyer.BonusBalance < request.Amount)
            {
                return new TransactionResultDto
                {
                    Success = false,
                    ErrorMessage = $"Buyer does not have enough bonus balance. Current balance: {buyer.BonusBalance}, Requested: {request.Amount}"
                };
            }

            // Create the transaction
            var transaction = new TransactionDto
            {
                Id = Guid.NewGuid(),
                UserId = request.BuyerId,
                StoreId = request.StoreId,
                CompanyId = store.CompanyId,
                Amount = request.Amount,
                Type = request.Type,
                Timestamp = DateTime.UtcNow,
                Status = TransactionStatus.Completed,
                Description = $"Transaction processed by seller {sellerId} at store {store.Name}"
            };

            await _transactionRepository.CreateAsync(transaction);

            // Update buyer's bonus balance
            decimal newBuyerBalance;
            if (request.Type == TransactionType.Earn)
            {
                // If earning, add to the buyer's balance
                newBuyerBalance = buyer.BonusBalance + request.Amount;
            }
            else
            {
                // If spending, subtract from the buyer's balance
                newBuyerBalance = buyer.BonusBalance - request.Amount;
            }
            await _userRepository.UpdateBalanceAsync(request.BuyerId, newBuyerBalance);

            // Update company's bonus balance
            var company = await _companyRepository.GetByIdAsync(store.CompanyId);
            if (company != null)
            {
                decimal newCompanyBalance;
                if (request.Type == TransactionType.Earn)
                {
                    // If buyer is earning, company is spending
                    newCompanyBalance = company.BonusBalance - request.Amount;
                }
                else
                {
                    // If buyer is spending, company is earning
                    newCompanyBalance = company.BonusBalance + request.Amount;
                }
                await _companyRepository.UpdateBalanceAsync(store.CompanyId, newCompanyBalance);
            }

            return new TransactionResultDto
            {
                Success = true,
                Transaction = transaction
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing transaction by seller {SellerId}", sellerId);
            return new TransactionResultDto
            {
                Success = false,
                ErrorMessage = $"An error occurred while processing the transaction: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// Confirm a transaction return
    /// </summary>
    public async Task<bool> ConfirmTransactionReturnAsync(Guid sellerId, Guid transactionId)
    {
        await ValidateUserRoleAsync(sellerId, UserRole.Seller);

        try
        {
            var transaction = await _transactionRepository.GetByIdAsync(transactionId);
            if (transaction == null)
            {
                throw new KeyNotFoundException($"Transaction with ID {transactionId} not found");
            }

            if (transaction.Status != TransactionStatus.Completed)
            {
                throw new InvalidOperationException($"Transaction with status {transaction.Status} cannot be returned");
            }

            // Mark the transaction as returned
            await _transactionRepository.UpdateTransactionStatusAsync(transactionId, TransactionStatus.Reversed);

            // Update the buyer's balance
            if (transaction.UserId.HasValue)
            {
                var buyer = await _userRepository.GetByIdAsync(transaction.UserId.Value);
                if (buyer != null)
                {
                    decimal newBuyerBalance;
                    if (transaction.Type == TransactionType.Earn)
                    {
                        // If it was an earn, subtract from the buyer's balance
                        newBuyerBalance = buyer.BonusBalance - transaction.Amount;
                    }
                    else
                    {
                        // If it was a spend, add back to the buyer's balance
                        newBuyerBalance = buyer.BonusBalance + transaction.Amount;
                    }
                    await _userRepository.UpdateBalanceAsync(transaction.UserId.Value, newBuyerBalance);
                }
            }

            // Update the company's balance
            if (transaction.CompanyId.HasValue)
            {
                var company = await _companyRepository.GetByIdAsync(transaction.CompanyId.Value);
                if (company != null)
                {
                    decimal newCompanyBalance;
                    if (transaction.Type == TransactionType.Earn)
                    {
                        // If it was a buyer earn, add back to the company's balance
                        newCompanyBalance = company.BonusBalance + transaction.Amount;
                    }
                    else
                    {
                        // If it was a buyer spend, subtract from the company's balance
                        newCompanyBalance = company.BonusBalance - transaction.Amount;
                    }
                    await _companyRepository.UpdateBalanceAsync(transaction.CompanyId.Value, newCompanyBalance);
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error confirming transaction return {TransactionId} by seller {SellerId}", transactionId, sellerId);
            throw;
        }
    }

    /// <summary>
    /// Get a buyer's bonus balance
    /// </summary>
    public async Task<decimal> GetBuyerBonusBalanceAsync(Guid buyerId)
    {
        try
        {
            var buyer = await _userRepository.GetByIdAsync(buyerId);
            if (buyer == null)
            {
                throw new KeyNotFoundException($"Buyer with ID {buyerId} not found");
            }

            return buyer.BonusBalance;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting bonus balance for buyer {BuyerId}", buyerId);
            throw;
        }
    }

    /// <summary>
    /// Get a store's bonus balance
    /// </summary>
    public async Task<decimal> GetStoreBonusBalanceAsync(Guid storeId)
    {
        try
        {
            return await _storeRepository.GetStoreBonusBalanceAsync(storeId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting bonus balance for store {StoreId}", storeId);
            throw;
        }
    }

    /// <summary>
    /// Get store bonus transactions
    /// </summary>
    public async Task<IEnumerable<StoreBonusTransactionsDto>> GetStoreBonusTransactionsAsync(Guid storeId)
    {
        try
        {
            var store = await _storeRepository.GetByIdAsync(storeId);
            if (store == null)
            {
                throw new KeyNotFoundException($"Store with ID {storeId} not found");
            }

            var transactions = await _transactionRepository.GetTransactionsByStoreIdAsync(storeId);

            return new List<StoreBonusTransactionsDto>
            {
                new()
                {
                    StoreId = storeId,
                    StoreName = store.Name,
                    TotalTransactions = transactions.Sum(t => t.Amount),
                    Transactions = transactions.OrderByDescending(t => t.Timestamp).ToList()
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting bonus transactions for store {StoreId}", storeId);
            throw;
        }
    }
}