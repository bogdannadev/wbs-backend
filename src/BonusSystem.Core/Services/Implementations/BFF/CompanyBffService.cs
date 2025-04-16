using BonusSystem.Core.Repositories;
using BonusSystem.Core.Services.Interfaces;
using BonusSystem.Shared.Dtos;
using BonusSystem.Shared.Models;

namespace BonusSystem.Core.Services.Implementations.BFF;

public class CompanyBffService : ICompanyBffService
{
    private readonly IUserRepository _userRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly IStoreRepository _storeRepository;
    private readonly ITransactionRepository _transactionRepository;

    public CompanyBffService(
        IUserRepository userRepository,
        ICompanyRepository companyRepository,
        IStoreRepository storeRepository,
        ITransactionRepository transactionRepository)
    {
        _userRepository = userRepository;
        _companyRepository = companyRepository;
        _storeRepository = storeRepository;
        _transactionRepository = transactionRepository;
    }

    public async Task<UserContextDto> GetUserContextAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new ArgumentException($"User with ID {userId} not found", nameof(userId));
        }

        return new UserContextDto
        {
            UserId = user.Id,
            Username = user.Username,
            Role = user.Role,
            BonusBalance = user.BonusBalance
        };
    }

    public async Task<IEnumerable<PermittedActionDto>> GetPermittedActionsAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new ArgumentException($"User with ID {userId} not found", nameof(userId));
        }

        // Define permitted actions based on role
        var permittedActions = new List<PermittedActionDto>();

        if (user.Role == UserRole.Company || user.Role == UserRole.StoreAdmin)
        {
            permittedActions.Add(new PermittedActionDto
            {
                ActionName = "RegisterStore",
                Description = "Register a new store",
                Endpoint = "/api/company/stores"
            });

            permittedActions.Add(new PermittedActionDto
            {
                ActionName = "RegisterSeller",
                Description = "Register a new seller",
                Endpoint = "/api/company/sellers"
            });

            permittedActions.Add(new PermittedActionDto
            {
                ActionName = "ViewStatistics",
                Description = "View company statistics",
                Endpoint = "/api/company/statistics"
            });

            permittedActions.Add(new PermittedActionDto
            {
                ActionName = "ViewTransactions",
                Description = "View transaction summary",
                Endpoint = "/api/company/transactions"
            });
        }

        return permittedActions;
    }

    public async Task<bool> RegisterStore(StoreRegistrationDto storeDto)
    {
        // Validate company exists
        var company = await _companyRepository.GetByIdAsync(storeDto.CompanyId);
        if (company == null)
        {
            throw new ArgumentException($"Company with ID {storeDto.CompanyId} not found", nameof(storeDto.CompanyId));
        }

        // Check if company is active
        if (company.Status != CompanyStatus.Active)
        {
            throw new InvalidOperationException($"Company with ID {storeDto.CompanyId} is not active");
        }

        // Create store
        var store = new StoreDto
        {
            Id = Guid.NewGuid(),
            CompanyId = storeDto.CompanyId,
            Name = storeDto.Name,
            Location = storeDto.Location,
            Address = storeDto.Address,
            ContactPhone = storeDto.ContactPhone,
            Status = StoreStatus.PendingApproval // New stores require admin approval
        };

        await _storeRepository.CreateAsync(store);

        // Assign sellers to the store if any provided
        if (storeDto.SellerIds.Any())
        {
            foreach (var sellerId in storeDto.SellerIds)
            {
                var seller = await _userRepository.GetByIdAsync(sellerId);
                if (seller != null && seller.Role == UserRole.Seller)
                {
                    await _storeRepository.AddSellerToStoreAsync(store.Id, sellerId);
                }
            }
        }

        return true;
    }

    public async Task<bool> RegisterSeller(UserRegistrationDto seller)
    {
        // Check if email is already in use
        var existingUser = await _userRepository.GetByEmailAsync(seller.Email);
        if (existingUser != null)
        {
            throw new InvalidOperationException($"Email {seller.Email} is already in use");
        }

        // Ensure role is Seller
        if (seller.Role != UserRole.Seller)
        {
            throw new ArgumentException("User role must be Seller", nameof(seller.Role));
        }

        // Create new seller user
        var newSeller = new UserDto
        {
            Id = Guid.NewGuid(),
            Username = seller.Username,
            Email = seller.Email,
            Role = UserRole.Seller,
            BonusBalance = 0 // Sellers don't have bonus balance
        };

        await _userRepository.CreateAsync(newSeller);
        return true;
    }

    public async Task<DashboardStatisticsDto> GetStatisticsAsync(StatisticsQueryDto query)
    {
        // If companyId is provided, get stats for that company only
        if (query.CompanyId.HasValue)
        {
            var company = await _companyRepository.GetByIdAsync(query.CompanyId.Value);
            if (company == null)
            {
                throw new ArgumentException($"Company with ID {query.CompanyId} not found", nameof(query.CompanyId));
            }

            // Get stores for this company
            var stores = await _storeRepository.GetStoresByCompanyIdAsync(query.CompanyId.Value);
            
            // Get transactions for this company
            var transactions = await _transactionRepository.GetTransactionsByCompanyIdAsync(query.CompanyId.Value);
            
            // Filter by date range if provided
            if (query.StartDate.HasValue && query.EndDate.HasValue)
            {
                transactions = transactions.Where(t => 
                    t.Timestamp >= query.StartDate.Value && 
                    t.Timestamp <= query.EndDate.Value).ToList();
            }

            return new DashboardStatisticsDto
            {
                TotalBonusCirculation = company.OriginalBonusBalance,
                CurrentActiveBonus = company.BonusBalance,
                TotalTransactions = transactions.Count(),
                ActiveUsers = 0, // Not applicable for a single company
                ActiveCompanies = 1,
                ActiveStores = stores.Count(s => s.Status == StoreStatus.Active)
            };
        }
        else
        {
            // Get system-wide statistics
            decimal totalCirculation = await _transactionRepository.GetTotalBonusCirculationAsync();
            decimal activeBonus = await _transactionRepository.GetTotalActiveBonus();
            int transactionCount = await _transactionRepository.GetTotalTransactionsCountAsync();
            
            // Get active companies
            var activeCompanies = await _companyRepository.GetCompaniesByStatusAsync(CompanyStatus.Active);
            
            // Get active buyer users
            var activeUsers = (await _userRepository.GetUsersByRoleAsync(UserRole.Buyer)).Count();
            
            // Get all active stores
            var allStores = await _storeRepository.GetAllAsync();
            var activeStores = allStores.Count(s => s.Status == StoreStatus.Active);

            return new DashboardStatisticsDto
            {
                TotalBonusCirculation = totalCirculation,
                CurrentActiveBonus = activeBonus,
                TotalTransactions = transactionCount,
                ActiveUsers = activeUsers,
                ActiveCompanies = activeCompanies.Count(),
                ActiveStores = activeStores
            };
        }
    }

    public async Task<TransactionDto> GetTransactionSummaryAsync(Guid? companyId)
    {
        if (!companyId.HasValue)
        {
            throw new ArgumentNullException(nameof(companyId), "Company ID must be provided");
        }

        // Validate company exists
        var company = await _companyRepository.GetByIdAsync(companyId.Value);
        if (company == null)
        {
            throw new ArgumentException($"Company with ID {companyId} not found", nameof(companyId));
        }

        // Get most recent transaction for this company
        var transactions = await _transactionRepository.GetTransactionsByCompanyIdAsync(companyId.Value);
        var latestTransaction = transactions.OrderByDescending(t => t.Timestamp).FirstOrDefault();

        if (latestTransaction == null)
        {
            // If no transactions exist, return a placeholder
            return new TransactionDto
            {
                Id = Guid.Empty,
                CompanyId = companyId,
                BonusAmount = 0,
                TotalCost = 0,
                Type = TransactionType.AdminAdjustment,
                Timestamp = DateTime.UtcNow,
                Status = TransactionStatus.Completed,
                Description = "No transactions found"
            };
        }

        return latestTransaction;
    }
}