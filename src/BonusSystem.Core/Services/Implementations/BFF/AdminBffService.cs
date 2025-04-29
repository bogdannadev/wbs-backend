using BonusSystem.Core.Services.Interfaces;
using BonusSystem.Shared.Dtos;
using BonusSystem.Shared.Models;

namespace BonusSystem.Core.Services.Implementations.BFF;

/// <summary>
/// BFF service for Admin role
/// </summary>
public class AdminBffService : BaseBffService, IAdminBffService
{
    public AdminBffService(
        IDataService dataService,
        IAuthenticationService authService)
        : base(dataService, authService)
    {
    }

    /// <summary>
    /// Gets the permitted actions for an admin
    /// </summary>
    public override async Task<IEnumerable<PermittedActionDto>> GetPermittedActionsAsync(Guid userId)
    {
        var role = await _dataService.Users.GetUserRoleAsync(userId);
        if (role != UserRole.SystemAdmin)
        {
            return Enumerable.Empty<PermittedActionDto>();
        }

        return new List<PermittedActionDto>
        {
            new()
            {
                ActionName = "RegisterCompany", Description = "Register a new company",
                Endpoint = "/api/admin/companies"
            },
            new()
            {
                ActionName = "UpdateCompanyStatus", Description = "Update company status",
                Endpoint = "/api/admin/companies/{id}/status"
            },
            new()
            {
                ActionName = "ModerateStore", Description = "Moderate a store",
                Endpoint = "/api/admin/stores/{id}/moderate"
            },
            new()
            {
                ActionName = "CreditCompanyBalance", Description = "Credit company balance",
                Endpoint = "/api/admin/companies/{id}/credit"
            },
            new()
            {
                ActionName = "GetSystemTransactions", Description = "Get system transactions",
                Endpoint = "/api/admin/transactions"
            },
            new()
            {
                ActionName = "SendNotification", Description = "Send system notification",
                Endpoint = "/api/admin/notifications"
            }
        };
    }

    /// <summary>
    /// Registers a new company
    /// </summary>
    public async Task<CompanyRegistrationResultDto> RegisterCompanyAsync(CompanyRegistrationDto request)
    {
        // This method is kept for backward compatibility
        // It should not be used for new implementations
        var company = new CompanyDto
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            ContactEmail = request.ContactEmail,
            ContactPhone = request.ContactPhone,
            BonusBalance = request.InitialBonusBalance,
            OriginalBonusBalance = request.InitialBonusBalance,
            Status = CompanyStatus.Active,
            CreatedAt = DateTime.UtcNow,
            Stores = new List<StoreDto>()
        };

        // Save the company
        await _dataService.Companies.CreateAsync(company);

        return new CompanyRegistrationResultDto
        {
            Success = true,
            Company = company
        };
    }
    
    /// <summary>
    /// Registers a new company with its admin user
    /// </summary>
    public async Task<CompanyRegistrationResultDto> RegisterCompanyWithAdminAsync(CompanyWithAdminRegistrationDto request)
    {
        try
        {
            // Check if admin email is already in use
            var existingUser = await _dataService.Users.GetByEmailAsync(request.AdminEmail);
            if (existingUser != null)
            {
                return new CompanyRegistrationResultDto
                {
                    Success = false,
                    ErrorMessage = $"Email {request.AdminEmail} is already in use"
                };
            }
            
            // Create the company
            var company = new CompanyDto
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                ContactEmail = request.ContactEmail,
                ContactPhone = request.ContactPhone,
                BonusBalance = request.InitialBonusBalance,
                OriginalBonusBalance = request.InitialBonusBalance,
                Status = CompanyStatus.Active,
                CreatedAt = DateTime.UtcNow,
                Stores = new List<StoreDto>()
            };

            // Create company admin user
            var companyAdmin = new UserDto
            {
                Id = Guid.NewGuid(),
                Username = request.AdminUsername,
                Email = request.AdminEmail,
                Role = UserRole.Company,
                BonusBalance = 0,
                CompanyId = company.Id // Link to the company
            };
            
            // Save the company
            await _dataService.Companies.CreateAsync(company);
            
            // Save the company admin
            await _dataService.Users.CreateAsync(companyAdmin);
            
            // Create password hash for the admin
            var authResult = await _authService.SignUpAsync(new UserRegistrationDto
            {
                Username = request.AdminUsername,
                Email = request.AdminEmail,
                Password = request.AdminPassword,
                Role = UserRole.Company
            });
            
            if (!authResult.Success)
            {
                return new CompanyRegistrationResultDto
                {
                    Success = false,
                    ErrorMessage = authResult.ErrorMessage ?? "Failed to create admin user"
                };
            }

            return new CompanyRegistrationResultDto
            {
                Success = true,
                Company = company
            };
        }
        catch (Exception ex)
        {
            return new CompanyRegistrationResultDto
            {
                Success = false,
                ErrorMessage = $"Error creating company: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// Updates the status of a company
    /// </summary>
    public async Task<bool> UpdateCompanyStatusAsync(Guid companyId, CompanyStatus status)
    {
        return await _dataService.Companies.UpdateStatusAsync(companyId, status);
    }

    /// <summary>
    /// Moderates a store (approves or rejects)
    /// </summary>
    public async Task<bool> ModerateStoreAsync(Guid storeId, bool isApproved)
    {
        var store = await _dataService.Stores.GetByIdAsync(storeId);
        if (store == null || store.Status != StoreStatus.PendingApproval)
        {
            return false;
        }

        var newStatus = isApproved ? StoreStatus.Active : StoreStatus.Inactive;
        return await _dataService.Stores.UpdateStatusAsync(storeId, newStatus);
    }

    /// <summary>
    /// Credits a company's bonus balance
    /// </summary>
    public async Task<bool> CreditCompanyBalanceAsync(Guid companyId, decimal amount)
    {
        var company = await _dataService.Companies.GetByIdAsync(companyId);
        if (company == null)
        {
            return false;
        }

        // Update company balance
        var newBalance = company.BonusBalance + amount;
        var success = await _dataService.Companies.UpdateBalanceAsync(companyId, newBalance);

        if (success)
        {
            // Create an admin adjustment transaction
            var transaction = new TransactionDto
            {
                Id = Guid.NewGuid(),
                CompanyId = companyId,
                BonusAmount = amount,
                TotalCost = 0.00m,
                Type = TransactionType.AdminAdjustment,
                Timestamp = DateTime.UtcNow,
                Status = TransactionStatus.Completed,
                Description = "Admin credit adjustment"
            };

            await _dataService.Transactions.CreateAsync(transaction);

            // Update original balance if needed
            var originalBalance = await _dataService.Companies.GetOriginalBalanceAsync(companyId);
            if (originalBalance < newBalance)
            {
                await _dataService.Companies.ResetToOriginalBalanceAsync(companyId);
            }
        }

        return success;
    }

    /// <summary>
    /// Gets system transactions
    /// </summary>
    public async Task<IEnumerable<TransactionDto>> GetSystemTransactionsAsync(Guid? companyId = null,
        DateTime? startDate = null, DateTime? endDate = null)
    {
        IEnumerable<TransactionDto> transactions;

        if (companyId.HasValue)
        {
            transactions = await _dataService.Transactions.GetTransactionsByCompanyIdAsync(companyId.Value);
        }
        else
        {
            transactions = await _dataService.Transactions.GetAllAsync();
        }

        // Apply date filters if provided
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

    /// <summary>
    /// Sends a system notification
    /// </summary>
    public async Task<bool> SendSystemNotificationAsync(Guid? recipientId, string message, NotificationType type)
    {
        if (recipientId.HasValue)
        {
            // Send to specific recipient
            return await _dataService.Notifications.SendNotificationAsync(recipientId.Value, message, type);
        }
        else
        {
            // Send to all users of a specific role based on notification type
            UserRole targetRole = type switch
            {
                NotificationType.Transaction => UserRole.Buyer,
                NotificationType.AdminMessage => UserRole.SystemAdmin,
                _ => UserRole.Buyer // Default to buyers for other types
            };

            return await _dataService.Notifications.SendNotificationToRoleAsync(targetRole, message, type);
        }
    }

    public async Task<List<CompanyFeeResult>> GetTransactionFeesAsync(TransactionFeeRequest request)
    {
        var transactions = await _dataService.Transactions.GetAllAsync();

        if (request.FromDate != null)
            transactions = transactions.Where(t => t.Timestamp >= request.FromDate);

        if (request.EndDate != null)
            transactions = transactions.Where(t => t.Timestamp <= request.EndDate);


        return transactions
            .Where(t => t.CompanyId.HasValue)
            .Where(t => t.Status == TransactionStatus.Completed)
            .GroupBy(t => t.CompanyId!.Value)
            .Select(g => new CompanyFeeResult
            {
                CompanyId = g.Key,
                TotalTransactions = g.Count(),
                TotalFee = g.Sum(t => t.TotalCost * request.FeePercent)
            })
            .ToList();
    }
}