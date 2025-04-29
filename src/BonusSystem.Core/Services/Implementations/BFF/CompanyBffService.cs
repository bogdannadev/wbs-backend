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
            
            permittedActions.Add(new PermittedActionDto
            {
                ActionName = "ViewStoresWithSellers",
                Description = "View company stores with sellers",
                Endpoint = "/api/companies/stores-with-sellers"
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
        // This method is kept for backward compatibility
        // It should not be used for new implementations
        throw new NotSupportedException("This method is deprecated. Use RegisterSellerForCompany instead to ensure sellers are properly linked to a company.");
    }
    
    public async Task<bool> RegisterSellerForCompany(SellerRegistrationDto seller, Guid companyId)
    {
        // Validate company exists
        var company = await _companyRepository.GetByIdAsync(companyId);
        if (company == null)
        {
            throw new ArgumentException($"Company with ID {companyId} not found", nameof(companyId));
        }
        
        // Check if company is active
        if (company.Status != CompanyStatus.Active)
        {
            throw new InvalidOperationException($"Company with ID {companyId} is not active");
        }
        
        // Check if email is already in use
        var existingUser = await _userRepository.GetByEmailAsync(seller.Email);
        if (existingUser != null)
        {
            throw new InvalidOperationException($"Email {seller.Email} is already in use");
        }

        // Create new seller user with company association
        var newSeller = new UserDto
        {
            Id = Guid.NewGuid(),
            Username = seller.Username,
            Email = seller.Email,
            Role = UserRole.Seller,
            BonusBalance = 0, // Sellers don't have bonus balance
            CompanyId = companyId // Link to the company
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

    public async Task<StoresWithSellersPagedResponseDto> GetStoresWithSellersAsync(Guid companyId, StoresFilterRequestDto filter)
    {
        // Validate company exists
        var company = await _companyRepository.GetByIdAsync(companyId);
        if (company == null)
        {
            throw new ArgumentException($"Company with ID {companyId} not found", nameof(companyId));
        }

        // Validate filter parameters
        if (filter.StoreStatus.HasValue)
        {
            if (!Enum.IsDefined(typeof(StoreStatus), filter.StoreStatus.Value))
            {
                throw new ArgumentException($"Invalid store status value: {filter.StoreStatus.Value}", nameof(filter.StoreStatus));
            }
        }

        if (filter.SellerRole.HasValue)
        {
            if (!Enum.IsDefined(typeof(UserRole), filter.SellerRole.Value))
            {
                throw new ArgumentException($"Invalid seller role value: {filter.SellerRole.Value}", nameof(filter.SellerRole));
            }
            
            // Ensure we're only filtering for Seller role
            if (filter.SellerRole.Value != UserRole.Seller)
            {
                filter = filter with { SellerRole = UserRole.Seller };
            }
        }

        // Get all stores for this company
        var allStores = await _storeRepository.GetStoresByCompanyIdAsync(companyId);
        
        // Apply store status filter if provided
        if (filter.StoreStatus.HasValue)
        {
            allStores = allStores.Where(s => s.Status == filter.StoreStatus.Value).ToList();
        }
        
        // Calculate total count before pagination
        var totalCount = allStores.Count();
        
        // Calculate total pages
        var totalPages = (int)Math.Ceiling((double)totalCount / filter.PageSize);
        
        // Apply pagination
        var paginatedStores = allStores
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToList();
        
        // Create list to hold stores with sellers
        var storesWithSellers = new List<StoreWithSellersDto>();
        
        // Get all sellers for all paginated stores in a single operation to avoid N+1 queries
        var storeIds = paginatedStores.Select(s => s.Id).ToList();
        Dictionary<Guid, List<UserDto>> sellersByStore = new();
        
        // Prepare a dictionary to hold sellers by store ID
        foreach (var storeId in storeIds)
        {
            var storeSellers = await _storeRepository.GetSellersByStoreIdAsync(storeId);
            
            // We know sellers should have Seller role, but double-check for safety
            sellersByStore[storeId] = storeSellers.Where(s => s.Role == UserRole.Seller).ToList();
        }
        
        // For each store, add it with its sellers to the result
        foreach (var store in paginatedStores)
        {
            var sellers = sellersByStore.ContainsKey(store.Id) ? sellersByStore[store.Id] : new List<UserDto>();
            
            storesWithSellers.Add(new StoreWithSellersDto
            {
                Id = store.Id,
                CompanyId = store.CompanyId,
                Name = store.Name,
                Location = store.Location,
                Address = store.Address,
                ContactPhone = store.ContactPhone,
                Status = store.Status,
                Sellers = sellers
            });
        }
        
        // Create and return the paginated response
        return new StoresWithSellersPagedResponseDto
        {
            Stores = storesWithSellers,
            TotalCount = totalCount,
            Page = filter.Page,
            PageSize = filter.PageSize,
            TotalPages = totalPages
        };
    }
}