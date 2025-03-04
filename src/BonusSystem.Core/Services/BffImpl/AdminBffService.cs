using BonusSystem.Core.Repositories;
using BonusSystem.Core.Services.Interfaces;
using BonusSystem.Shared.Dtos;
using BonusSystem.Shared.Models;
using Microsoft.Extensions.Logging;

namespace BonusSystem.Core.Services.BffImpl;

/// <summary>
/// Implementation of BFF service for Admin users
/// </summary>
public class AdminBffService : BaseBffService, IAdminBffService
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IStoreRepository _storeRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly INotificationRepository _notificationRepository;

    public AdminBffService(
        IUserRepository userRepository,
        ICompanyRepository companyRepository,
        IStoreRepository storeRepository,
        ITransactionRepository transactionRepository,
        INotificationRepository notificationRepository,
        ILogger<AdminBffService> logger) 
        : base(userRepository, logger)
    {
        _companyRepository = companyRepository;
        _storeRepository = storeRepository;
        _transactionRepository = transactionRepository;
        _notificationRepository = notificationRepository;
    }

    /// <summary>
    /// Get admin-specific permitted actions
    /// </summary>
    public override async Task<IEnumerable<PermittedActionDto>> GetPermittedActionsAsync(Guid userId)
    {
        await ValidateUserRoleAsync(userId, UserRole.SystemAdmin);

        return new List<PermittedActionDto>
        {
            new() { ActionName = "RegisterCompany", Description = "Register a new company", Endpoint = "/api/admin/companies" },
            new() { ActionName = "UpdateCompanyStatus", Description = "Update company status", Endpoint = "/api/admin/companies/{id}/status" },
            new() { ActionName = "ModerateStore", Description = "Moderate a store", Endpoint = "/api/admin/stores/{id}/moderate" },
            new() { ActionName = "CreditCompanyBalance", Description = "Credit company bonus balance", Endpoint = "/api/admin/companies/{id}/credit" },
            new() { ActionName = "GetSystemTransactions", Description = "Get system-wide transactions", Endpoint = "/api/admin/transactions" },
            new() { ActionName = "SendSystemNotification", Description = "Send system notification", Endpoint = "/api/admin/notifications" }
        };
    }

    /// <summary>
    /// Register a new company
    /// </summary>
    public async Task<CompanyRegistrationResultDto> RegisterCompanyAsync(CompanyRegistrationDto request)
    {
        try
        {
            var companyId = Guid.NewGuid();
            var company = new CompanyDto
            {
                Id = companyId,
                Name = request.Name,
                ContactEmail = request.ContactEmail,
                ContactPhone = request.ContactPhone,
                BonusBalance = request.InitialBonusBalance,
                OriginalBonusBalance = request.InitialBonusBalance,
                Status = CompanyStatus.Active,
                CreatedAt = DateTime.UtcNow,
                Stores = new List<StoreDto>()
            };

            await _companyRepository.CreateAsync(company);

            return new CompanyRegistrationResultDto
            {
                Success = true,
                Company = company
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering company {CompanyName}", request.Name);
            return new CompanyRegistrationResultDto
            {
                Success = false,
                ErrorMessage = $"Error registering company: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// Update a company's status
    /// </summary>
    public async Task<bool> UpdateCompanyStatusAsync(Guid companyId, CompanyStatus status)
    {
        try
        {
            var company = await _companyRepository.GetByIdAsync(companyId);
            if (company == null)
            {
                throw new KeyNotFoundException($"Company with ID {companyId} not found");
            }

            await _companyRepository.UpdateStatusAsync(companyId, status);

            // If company is suspended, notify all users associated with it
            if (status == CompanyStatus.Suspended)
            {
                // In a real implementation, we would find all users associated with the company
                // and send them notifications
                await _notificationRepository.SendNotificationToRoleAsync(
                    UserRole.StoreAdmin,
                    $"Company {company.Name} has been suspended",
                    NotificationType.AdminMessage);
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating status for company {CompanyId} to {Status}", companyId, status);
            throw;
        }
    }

    /// <summary>
    /// Moderate a store (approve or reject)
    /// </summary>
    public async Task<bool> ModerateStoreAsync(Guid storeId, bool isApproved)
    {
        try
        {
            var store = await _storeRepository.GetByIdAsync(storeId);
            if (store == null)
            {
                throw new KeyNotFoundException($"Store with ID {storeId} not found");
            }

            StoreStatus newStatus = isApproved ? StoreStatus.Active : StoreStatus.Inactive;
            await _storeRepository.UpdateStatusAsync(storeId, newStatus);

            // Notify the company about the moderation result
            if (store.CompanyId != Guid.Empty)
            {
                var company = await _companyRepository.GetByIdAsync(store.CompanyId);
                if (company != null)
                {
                    string message = isApproved
                        ? $"Store {store.Name} has been approved"
                        : $"Store {store.Name} has been rejected";

                    // In a real implementation, we would notify the company's admin
                    // Here we're just simulating it
                    await _notificationRepository.SendNotificationAsync(
                        store.CompanyId,
                        message,
                        NotificationType.AdminMessage);
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error moderating store {StoreId}, approved: {IsApproved}", storeId, isApproved);
            throw;
        }
    }

    /// <summary>
    /// Credit a company's bonus balance
    /// </summary>
    public async Task<bool> CreditCompanyBalanceAsync(Guid companyId, decimal amount)
    {
        try
        {
            var company = await _companyRepository.GetByIdAsync(companyId);
            if (company == null)
            {
                throw new KeyNotFoundException($"Company with ID {companyId} not found");
            }

            // Update the company's bonus balance
            decimal newBalance = company.BonusBalance + amount;
            await _companyRepository.UpdateBalanceAsync(companyId, newBalance);

            // Update the company's original bonus balance if adding funds
            if (amount > 0)
            {
                decimal newOriginalBalance = company.OriginalBonusBalance + amount;
                company = company with
                {
                    OriginalBonusBalance = newOriginalBalance,
                    BonusBalance = newBalance
                };
                await _companyRepository.UpdateAsync(company);
            }

            // Create an admin adjustment transaction
            var transaction = new TransactionDto
            {
                Id = Guid.NewGuid(),
                CompanyId = companyId,
                Amount = amount,
                Type = TransactionType.AdminAdjustment,
                Timestamp = DateTime.UtcNow,
                Status = TransactionStatus.Completed,
                Description = $"Admin credit adjustment of {amount} to company {company.Name}"
            };

            await _transactionRepository.CreateAsync(transaction);

            // Notify the company about the balance adjustment
            await _notificationRepository.SendNotificationAsync(
                companyId,
                $"Your company's bonus balance has been adjusted by {amount}",
                NotificationType.AdminMessage);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error crediting company {CompanyId} with amount {Amount}", companyId, amount);
            throw;
        }
    }

    /// <summary>
    /// Get system-wide transactions with optional filtering
    /// </summary>
    public async Task<IEnumerable<TransactionDto>> GetSystemTransactionsAsync(Guid? companyId = null, DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            IEnumerable<TransactionDto> transactions;

            if (companyId.HasValue)
            {
                // Get transactions for a specific company
                transactions = await _transactionRepository.GetTransactionsByCompanyIdAsync(companyId.Value);
            }
            else
            {
                // Get all transactions
                transactions = await _transactionRepository.GetAllAsync();
            }

            // Apply date filtering if dates are provided
            if (startDate.HasValue)
            {
                transactions = transactions.Where(t => t.Timestamp >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                transactions = transactions.Where(t => t.Timestamp <= endDate.Value);
            }

            return transactions.OrderByDescending(t => t.Timestamp);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving system transactions for companyId: {CompanyId}, startDate: {StartDate}, endDate: {EndDate}",
                companyId, startDate, endDate);
            throw;
        }
    }

    /// <summary>
    /// Send a system notification to specific users or roles
    /// </summary>
    public async Task<bool> SendSystemNotificationAsync(Guid? recipientId, string message, NotificationType type)
    {
        try
        {
            if (recipientId.HasValue)
            {
                // Send to a specific recipient
                return await _notificationRepository.SendNotificationAsync(recipientId.Value, message, type);
            }
            else
            {
                // Send to all users
                // In a real implementation, we would have a way to get all user IDs
                // Here we're sending to all roles instead
                bool buyerResult = await _notificationRepository.SendNotificationToRoleAsync(UserRole.Buyer, message, type);
                bool sellerResult = await _notificationRepository.SendNotificationToRoleAsync(UserRole.Seller, message, type);
                bool storeAdminResult = await _notificationRepository.SendNotificationToRoleAsync(UserRole.StoreAdmin, message, type);
                bool observerResult = await _notificationRepository.SendNotificationToRoleAsync(UserRole.CompanyObserver, message, type);

                return buyerResult && sellerResult && storeAdminResult && observerResult;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending system notification to recipientId: {RecipientId}, message: {Message}, type: {Type}",
                recipientId, message, type);
            throw;
        }
    }
}