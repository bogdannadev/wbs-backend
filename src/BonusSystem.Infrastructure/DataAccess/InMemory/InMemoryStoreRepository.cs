using BonusSystem.Core.Repositories;
using BonusSystem.Shared.Dtos;
using BonusSystem.Shared.Models;

namespace BonusSystem.Infrastructure.DataAccess.InMemory;

/// <summary>
/// In-memory implementation of the store repository
/// </summary>
public class InMemoryStoreRepository : InMemoryRepository<StoreDto, Guid>, IStoreRepository
{
    private readonly Dictionary<Guid, List<Guid>> _storeSellers = new();

    public InMemoryStoreRepository() : base(store => store.Id)
    {
        // Initialize with some sample data
        var stores = new List<StoreDto>
        {
            new()
            {
                Id = Guid.Parse("77777777-7777-7777-7777-777777777777"),
                CompanyId = Guid.Parse("44444444-4444-4444-4444-444444444444"), // Alpha Store Chain
                Name = "Alpha Downtown",
                Location = "Downtown",
                Address = "123 Main St, Downtown",
                ContactPhone = "+1234567890",
                Status = StoreStatus.Active
            },
            new()
            {
                Id = Guid.Parse("88888888-8888-8888-8888-888888888888"),
                CompanyId = Guid.Parse("44444444-4444-4444-4444-444444444444"), // Alpha Store Chain
                Name = "Alpha Uptown",
                Location = "Uptown",
                Address = "456 Park Ave, Uptown",
                ContactPhone = "+1234567891",
                Status = StoreStatus.Active
            },
            new()
            {
                Id = Guid.Parse("99999999-9999-9999-9999-999999999999"),
                CompanyId = Guid.Parse("55555555-5555-5555-5555-555555555555"), // Beta Retail Group
                Name = "Beta Central",
                Location = "Midtown",
                Address = "789 Center Blvd, Midtown",
                ContactPhone = "+1987654320",
                Status = StoreStatus.Active
            },
            new()
            {
                Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                CompanyId = Guid.Parse("66666666-6666-6666-6666-666666666666"), // Gamma Markets
                Name = "Gamma Express",
                Location = "Riverside",
                Address = "321 River Rd, Riverside",
                ContactPhone = "+1122334450",
                Status = StoreStatus.PendingApproval
            }
        };

        foreach (var store in stores)
        {
            _entities[store.Id] = store;
            _storeSellers[store.Id] = new List<Guid>();
        }

        // Assign seller to store
        var sellerId = Guid.Parse("22222222-2222-2222-2222-222222222222"); // seller1 from UserRepository
        AddSellerToStoreAsync(Guid.Parse("77777777-7777-7777-7777-777777777777"), sellerId)
            .Wait(); // Add seller1 to Alpha Downtown
    }

    public Task<IEnumerable<StoreDto>> GetStoresByCompanyIdAsync(Guid companyId)
    {
        var stores = _entities.Values.Where(s => s.CompanyId == companyId);
        return Task.FromResult(stores);
    }

    public Task<bool> UpdateStatusAsync(Guid storeId, StoreStatus status)
    {
        if (!_entities.TryGetValue(storeId, out var store))
        {
            return Task.FromResult(false);
        }

        var updatedStore = store with { Status = status };
        return Task.FromResult(_entities.TryUpdate(storeId, updatedStore, store));
    }

    public Task<bool> AddSellerToStoreAsync(Guid storeId, Guid sellerId)
    {
        if (!_entities.ContainsKey(storeId) || !_storeSellers.ContainsKey(storeId))
        {
            return Task.FromResult(false);
        }

        if (!_storeSellers[storeId].Contains(sellerId))
        {
            _storeSellers[storeId].Add(sellerId);
        }

        return Task.FromResult(true);
    }

    public Task<bool> RemoveSellerFromStoreAsync(Guid storeId, Guid sellerId)
    {
        if (!_entities.ContainsKey(storeId) || !_storeSellers.ContainsKey(storeId))
        {
            return Task.FromResult(false);
        }

        return Task.FromResult(_storeSellers[storeId].Remove(sellerId));
    }

    public Task<IEnumerable<UserDto>> GetSellersByStoreIdAsync(Guid storeId)
    {
        // In a real implementation, we would query the user repository
        // For now, just return empty list since we don't have access to IUserRepository here
        return Task.FromResult(Enumerable.Empty<UserDto>());
    }

    public Task<IEnumerable<StoreDto>> GetStoresByCategoryAsync(string category)
    {
        var stores = _entities.Values.Where(s => s.Status == StoreStatus.Active);
        return Task.FromResult(stores);
    }

    public Task<decimal> GetStoreBonusBalanceAsync(Guid storeId)
    {
        // In a real implementation, this would be calculated from transactions
        // For prototype, return a random value
        return Task.FromResult(10000m);
    }
}