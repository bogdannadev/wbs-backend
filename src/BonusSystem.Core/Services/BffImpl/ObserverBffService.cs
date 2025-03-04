using BonusSystem.Core.Repositories;
using BonusSystem.Core.Services.Interfaces;
using BonusSystem.Shared.Dtos;
using BonusSystem.Shared.Models;
using Microsoft.Extensions.Logging;

namespace BonusSystem.Core.Services.BffImpl;

/// <summary>
/// Implementation of BFF service for Observer users
/// </summary>
public class ObserverBffService : BaseBffService, IObserverBffService
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IStoreRepository _storeRepository;
    private readonly ITransactionRepository _transactionRepository;

    public ObserverBffService(
        IUserRepository userRepository,
        ICompanyRepository companyRepository,
        IStoreRepository storeRepository,
        ITransactionRepository transactionRepository,
        ILogger<ObserverBffService> logger)
        : base(userRepository, logger)
    {
        _companyRepository = companyRepository;
        _storeRepository = storeRepository;
        _transactionRepository = transactionRepository;
    }

    /// <summary>
    /// Get observer-specific permitted actions
    /// </summary>
    public override async Task<IEnumerable<PermittedActionDto>> GetPermittedActionsAsync(Guid userId)
    {
        var userRole = await _userRepository.GetUserRoleAsync(userId);
        
        // Validate that the user is either a company observer or system observer
        if (userRole != UserRole.CompanyObserver && userRole != UserRole.SystemObserver)
        {
            throw new UnauthorizedAccessException("User is not an observer");
        }

        var actions = new List<PermittedActionDto>
        {
            new() { ActionName = "GetStatistics", Description = "Get system statistics", Endpoint = "/api/observer/statistics" },
            new() { ActionName = "GetCompaniesOverview", Description = "Get companies overview", Endpoint = "/api/observer/companies" }
        };

        // System observers have additional permissions
        if (userRole == UserRole.SystemObserver)
        {
            actions.Add(new() { ActionName = "GetTransactionSummary", Description = "Get transaction summary", Endpoint = "/api/observer/transactions/summary" });
        }

        return actions;
    }

    /// <summary>
    /// Get system statistics based on query parameters
    /// </summary>
    public async Task<DashboardStatisticsDto> GetStatisticsAsync(StatisticsQueryDto query)
    {
        try
        {
            // Get all active users
            var users = await _userRepository.GetAllAsync();
            int activeUsers = users.Count();

            // Get all active companies
            var companies = await _companyRepository.GetAllAsync();
            int activeCompanies = companies.Count(c => c.Status == CompanyStatus.Active);

            // Get all active stores
            var stores = new List<StoreDto>();
            foreach (var company in companies)
            {
                var companyStores = await _storeRepository.GetStoresByCompanyIdAsync(company.Id);
                stores.AddRange(companyStores);
            }
            int activeStores = stores.Count(s => s.Status == StoreStatus.Active);

            // Get total bonus circulation
            decimal totalBonusCirculation = await _transactionRepository.GetTotalBonusCirculationAsync();

            // Get current active bonus
            decimal currentActiveBonus = await _transactionRepository.GetTotalActiveBonus();

            // Get total transaction count
            int totalTransactions = await _transactionRepository.GetTotalTransactionsCountAsync();

            // If company ID is specified, filter statistics for that company
            if (query.CompanyId.HasValue)
            {
                var company = await _companyRepository.GetByIdAsync(query.CompanyId.Value);
                if (company == null)
                {
                    throw new KeyNotFoundException($"Company with ID {query.CompanyId} not found");
                }

                var companyStores = await _storeRepository.GetStoresByCompanyIdAsync(query.CompanyId.Value);
                activeStores = companyStores.Count(s => s.Status == StoreStatus.Active);

                // Get company transactions
                var companyTransactions = await _transactionRepository.GetTransactionsByCompanyIdAsync(query.CompanyId.Value);
                
                // Apply date filtering if specified
                if (query.StartDate.HasValue)
                {
                    companyTransactions = companyTransactions.Where(t => t.Timestamp >= query.StartDate.Value);
                }
                if (query.EndDate.HasValue)
                {
                    companyTransactions = companyTransactions.Where(t => t.Timestamp <= query.EndDate.Value);
                }

                totalTransactions = companyTransactions.Count();
                totalBonusCirculation = companyTransactions.Sum(t => t.Amount);
                currentActiveBonus = company.BonusBalance;
                activeCompanies = 1;
                activeUsers = companyTransactions.Select(t => t.UserId).Distinct().Count();
            }
            else
            {
                // Apply date filtering if specified
                if (query.StartDate.HasValue || query.EndDate.HasValue)
                {
                    var allTransactions = await _transactionRepository.GetAllAsync();
                    
                    if (query.StartDate.HasValue)
                    {
                        allTransactions = allTransactions.Where(t => t.Timestamp >= query.StartDate.Value);
                    }
                    if (query.EndDate.HasValue)
                    {
                        allTransactions = allTransactions.Where(t => t.Timestamp <= query.EndDate.Value);
                    }

                    totalTransactions = allTransactions.Count();
                    totalBonusCirculation = allTransactions.Sum(t => t.Amount);
                }
            }

            return new DashboardStatisticsDto
            {
                TotalBonusCirculation = totalBonusCirculation,
                CurrentActiveBonus = currentActiveBonus,
                TotalTransactions = totalTransactions,
                ActiveUsers = activeUsers,
                ActiveCompanies = activeCompanies,
                ActiveStores = activeStores
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting statistics with query {CompanyId}, {StartDate}, {EndDate}", 
                query.CompanyId, query.StartDate, query.EndDate);
            throw;
        }
    }

    /// <summary>
    /// Get transaction summary for a company or the entire system
    /// </summary>
    public async Task<TransactionDto> GetTransactionSummaryAsync(Guid? companyId)
    {
        try
        {
            // Validate user is a system observer if no company ID is specified
            // This would typically be done in a real implementation

            IEnumerable<TransactionDto> transactions;
            string description;

            if (companyId.HasValue)
            {
                var company = await _companyRepository.GetByIdAsync(companyId.Value);
                if (company == null)
                {
                    throw new KeyNotFoundException($"Company with ID {companyId} not found");
                }

                transactions = await _transactionRepository.GetTransactionsByCompanyIdAsync(companyId.Value);
                description = $"Transaction summary for company {company.Name}";
            }
            else
            {
                transactions = await _transactionRepository.GetAllAsync();
                description = "Transaction summary for the entire system";
            }

            // Calculate sum of all transactions
            decimal totalAmount = transactions.Sum(t => t.Amount);

            // For the prototype, we'll create a simple transaction summary
            // In a real implementation, this would be a more detailed summary
            return new TransactionDto
            {
                Id = Guid.Empty, // Not a real transaction
                CompanyId = companyId,
                Amount = totalAmount,
                Type = TransactionType.AdminAdjustment, // Not a real transaction type
                Timestamp = DateTime.UtcNow,
                Status = TransactionStatus.Completed,
                Description = description
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting transaction summary for company {CompanyId}", companyId);
            throw;
        }
    }

    /// <summary>
    /// Get overview of all companies or filtered by status
    /// </summary>
    public async Task<IEnumerable<CompanySummaryDto>> GetCompaniesOverviewAsync()
    {
        try
        {
            var companies = await _companyRepository.GetAllAsync();
            var companySummaries = new List<CompanySummaryDto>();

            foreach (var company in companies)
            {
                // Get company transactions
                var transactions = await _transactionRepository.GetTransactionsByCompanyIdAsync(company.Id);
                decimal transactionVolume = transactions.Sum(t => t.Amount);

                // Get company stores
                var stores = await _storeRepository.GetStoresByCompanyIdAsync(company.Id);

                companySummaries.Add(new CompanySummaryDto
                {
                    Id = company.Id,
                    Name = company.Name,
                    BonusBalance = company.BonusBalance,
                    TransactionVolume = transactionVolume,
                    StoreCount = stores.Count(),
                    Status = company.Status
                });
            }

            return companySummaries;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting companies overview");
            throw;
        }
    }
}