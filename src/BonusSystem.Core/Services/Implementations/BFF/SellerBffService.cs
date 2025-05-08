using BonusSystem.Core.Repositories;
using BonusSystem.Core.Services.Interfaces;
using BonusSystem.Shared.Dtos;
using BonusSystem.Shared.Models;

namespace BonusSystem.Core.Services.Implementations.BFF;

/// <summary>
/// BFF service for Seller role
/// </summary>
public class SellerBffService : BaseBffService, ISellerBffService
{
    public SellerBffService(
        IDataService dataService,
        IAuthenticationService authService) 
        : base(dataService, authService)
    {
    }


    /// <summary>
    /// Gets the permitted actions for a seller
    /// </summary>
    public override async Task<IEnumerable<PermittedActionDto>> GetPermittedActionsAsync(Guid userId)
    {
        var role = await _dataService.Users.GetUserRoleAsync(userId);
        if (role != UserRole.Seller)
        {
            return Enumerable.Empty<PermittedActionDto>();
        }

        return new List<PermittedActionDto>
        {
            new() { ActionName = "ProcessTransaction", Description = "Process transaction", Endpoint = "/api/sellers/transactions" },
            new() { ActionName = "ConfirmReturn", Description = "Confirm transaction return", Endpoint = "/api/sellers/transactions/{id}/return" },
            new() { ActionName = "GetBuyerBalance", Description = "Get buyer's bonus balance", Endpoint = "/api/sellers/buyers/{id}/balance" },
            new() { ActionName = "GetStoreBalance", Description = "Get store's bonus balance", Endpoint = "/api/sellers/stores/{id}/balance" },
            new() { ActionName = "GetTransactions", Description = "Get store transactions", Endpoint = "/api/sellers/stores/{id}/transactions" }
        };
    }

    /// <summary>
    /// Processes a transaction
    /// </summary>
    public async Task<TransactionResultDto> ProcessTransactionAsync(Guid sellerId, TransactionRequestDto request, decimal cashbackpercent)
    {
        // Check if seller exists
        var seller = await _dataService.Users.GetByIdAsync(sellerId);
        if (seller == null || seller.Role != UserRole.Seller)
        {
            return new TransactionResultDto
            {
                Success = false,
                ErrorMessage = "Invalid seller"
            };
        }
        
        // Find seller's store
        var store = await _dataService.Stores.GetStoreBySellerIdAsync(sellerId);
        if (store == null || store.Status != StoreStatus.Active)
        {
            return new TransactionResultDto
            {
                Success = false,
                ErrorMessage = "Seller is not assigned to an active store"
            };
        }

        // Check if buyer exists
        var buyer = await _dataService.Users.GetByIdAsync(request.BuyerId);
        if (buyer == null || buyer.Role != UserRole.Buyer)
        {
            return new TransactionResultDto
            {
                Success = false,
                ErrorMessage = "Invalid buyer"
            };
        }

        // For spend transactions, check if buyer has enough balance
        if (request.Type == TransactionType.Spend && buyer.BonusBalance < request.BonusAmount)
        {
            return new TransactionResultDto
            {
                Success = false,
                ErrorMessage = "Insufficient bonus balance"
            };
        }
        decimal cashback = request.TotalCost * cashbackpercent; 

        // Create the transaction
        var transaction = new TransactionDto
        {
            Id = Guid.NewGuid(),
            UserId = request.BuyerId,
            CompanyId = store.CompanyId,
            StoreId = store.Id,
            BonusAmount = request.BonusAmount,
            TotalCost = request.TotalCost,
            Type = request.Type,
            Timestamp = DateTime.UtcNow,
            Status = TransactionStatus.Completed,
            CashbackAmount = cashback,
            Description = $"Transaction at {store.Name}"
        };

        var updatedBuyer = buyer with { BonusBalance = buyer.BonusBalance + cashback };
        await _dataService.Users.UpdateBalanceAsync(updatedBuyer.Id, updatedBuyer.BonusBalance);
        
        // Save the transaction
        await _dataService.Transactions.CreateAsync(transaction);

        // Update buyer's balance
        decimal newBuyerBalance = buyer.BonusBalance;
        if (request.Type == TransactionType.Earn)
        {
            newBuyerBalance += request.BonusAmount;
        }
        else if (request.Type == TransactionType.Spend)
        {
            newBuyerBalance -= request.BonusAmount;
        }

        await _dataService.Users.UpdateBalanceAsync(buyer.Id, newBuyerBalance);

        // Update company's balance
        var company = await _dataService.Companies.GetByIdAsync(store.CompanyId);
        if (company != null)
        {
            decimal newCompanyBalance = company.BonusBalance;
            if (request.Type == TransactionType.Earn)
            {
                newCompanyBalance -= request.BonusAmount;
            }
            else if (request.Type == TransactionType.Spend)
            {
                newCompanyBalance += request.BonusAmount;
            }

            await _dataService.Companies.UpdateBalanceAsync(company.Id, newCompanyBalance);
        }

        return new TransactionResultDto
        {
            Success = true,
            Transaction = transaction
        };
    }

    /// <summary>
    /// Confirms a transaction return
    /// </summary>
    public async Task<bool> ConfirmTransactionReturnAsync(Guid sellerId, Guid transactionId)
    {
        // Check if seller exists
        var seller = await _dataService.Users.GetByIdAsync(sellerId);
        if (seller == null || seller.Role != UserRole.Seller)
        {
            return false;
        }

        // Check if transaction exists
        var transaction = await _dataService.Transactions.GetByIdAsync(transactionId);
        if (transaction == null)
        {
            return false;
        }

        // Check if the transaction can be returned (e.g., not already reversed, not too old)
        if (transaction.Status != TransactionStatus.Completed || 
            transaction.Timestamp < DateTime.UtcNow.AddDays(-7))
        {
            return false;
        }

        // Update transaction status
        await _dataService.Transactions.UpdateTransactionStatusAsync(transactionId, TransactionStatus.Reversed);

        // Adjust buyer's balance if applicable
        if (transaction.UserId.HasValue)
        {
            var buyer = await _dataService.Users.GetByIdAsync(transaction.UserId.Value);
            if (buyer != null)
            {
                decimal newBalance = buyer.BonusBalance;
                if (transaction.Type == TransactionType.Earn)
                {
                    newBalance -= transaction.BonusAmount;
                }
                else if (transaction.Type == TransactionType.Spend)
                {
                    newBalance += transaction.BonusAmount;
                }

                await _dataService.Users.UpdateBalanceAsync(buyer.Id, newBalance);
            }
        }

        // Adjust company's balance if applicable
        if (transaction.CompanyId.HasValue)
        {
            var company = await _dataService.Companies.GetByIdAsync(transaction.CompanyId.Value);
            if (company != null)
            {
                decimal newBalance = company.BonusBalance;
                if (transaction.Type == TransactionType.Earn)
                {
                    newBalance += transaction.BonusAmount;
                }
                else if (transaction.Type == TransactionType.Spend)
                {
                    newBalance -= transaction.BonusAmount;
                }

                await _dataService.Companies.UpdateBalanceAsync(company.Id, newBalance);
            }
        }

        return true;
    }

    /// <summary>
    /// Gets the bonus balance for a buyer
    /// </summary>
    public async Task<decimal> GetBuyerBonusBalanceAsync(Guid buyerId)
    {
        var buyer = await _dataService.Users.GetByIdAsync(buyerId);
        return buyer?.BonusBalance ?? 0m;
    }

    /// <summary>
    /// Gets the bonus balance for a store
    /// </summary>
    public async Task<decimal> GetStoreBonusBalanceAsync(Guid storeId)
    {
        return await _dataService.Stores.GetStoreBonusBalanceAsync(storeId);
    }

    public async Task<decimal> GetStoreBonusBalanceByUserIdAsync(Guid userId)
    {
        var storeBySeller = await _dataService.Stores.GetStoreBySellerIdAsync(userId);
        
        return await GetStoreBonusBalanceAsync(storeBySeller.Id);
    }

    /// <summary>
    /// Gets the bonus transactions for a store
    /// </summary>
    public async Task<IEnumerable<StoreBonusTransactionsDto>> GetStoreBonusTransactionsAsync(Guid storeId)
    {
        var store = await _dataService.Stores.GetByIdAsync(storeId);
        if (store == null)
        {
            return [];
        }

        var transactions = await _dataService.Transactions.GetTransactionsByStoreIdAsync(storeId);
        var totalAmount = transactions.Sum(t => t.BonusAmount);

        var result = new StoreBonusTransactionsDto
        {
            StoreId = storeId,
            StoreName = store.Name,
            TotalTransactions = totalAmount,
            Transactions = transactions.OrderByDescending(t => t.Timestamp).ToList()
        };

        return [result];
    }

    public async Task<IEnumerable<StoreBonusTransactionsDto>> GetStoreBonusTransactionsByUserIdAsync(Guid userId)
    {
        var store = await _dataService.Stores.GetStoreBySellerIdAsync(userId);
        if (store == null)
        {
            return [];
        }

        return await GetStoreBonusTransactionsAsync(store.Id);
    }
}