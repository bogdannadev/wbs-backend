using BonusSystem.Core.Repositories;
using BonusSystem.Infrastructure.DataAccess.Postgres.Entities;
using BonusSystem.Shared.Dtos;
using BonusSystem.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BonusSystem.Infrastructure.DataAccess.Postgres.Repositories;

public class PostgresStoreRepository : IStoreRepository
{
    private readonly BonusSystemDbContext _dbContext;
    private readonly ILogger<PostgresStoreRepository> _logger;

    public PostgresStoreRepository(BonusSystemDbContext dbContext, ILogger<PostgresStoreRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<IEnumerable<StoreDto>> GetAllAsync()
    {
        var stores = await _dbContext.Stores
            .AsNoTracking()
            .OrderBy(s => s.Name)
            .ToListAsync();
            
        return stores.Select(MapToDto);
    }

    public async Task<StoreDto?> GetByIdAsync(Guid id)
    {
        var store = await _dbContext.Stores
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id);
            
        return store != null ? MapToDto(store) : null;
    }

    public async Task<Guid> CreateAsync(StoreDto entity)
    {
        var storeEntity = new StoreEntity
        {
            Id = Guid.NewGuid(),
            CompanyId = entity.CompanyId,
            Name = entity.Name,
            Location = entity.Location,
            Address = entity.Address,
            ContactPhone = entity.ContactPhone,
            Status = entity.Status,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        
        await _dbContext.Stores.AddAsync(storeEntity);
        await _dbContext.SaveChangesAsync();
        
        return storeEntity.Id;
    }

    public async Task<bool> UpdateAsync(StoreDto entity)
    {
        var storeEntity = await _dbContext.Stores
            .FirstOrDefaultAsync(s => s.Id == entity.Id);
            
        if (storeEntity == null)
            return false;
            
        storeEntity.Name = entity.Name;
        storeEntity.Location = entity.Location;
        storeEntity.Address = entity.Address;
        storeEntity.ContactPhone = entity.ContactPhone;
        storeEntity.Status = entity.Status;
        storeEntity.UpdatedAt = DateTime.UtcNow;
        
        _dbContext.Stores.Update(storeEntity);
        await _dbContext.SaveChangesAsync();
        
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var storeEntity = await _dbContext.Stores
            .FirstOrDefaultAsync(s => s.Id == id);
            
        if (storeEntity == null)
            return false;
            
        _dbContext.Stores.Remove(storeEntity);
        await _dbContext.SaveChangesAsync();
        
        return true;
    }

    public async Task<IEnumerable<StoreDto>> GetStoresByCompanyIdAsync(Guid companyId)
    {
        var stores = await _dbContext.Stores
            .AsNoTracking()
            .Where(s => s.CompanyId == companyId)
            .OrderBy(s => s.Name)
            .ToListAsync();
            
        return stores.Select(MapToDto);
    }

    public async Task<bool> UpdateStatusAsync(Guid storeId, StoreStatus status)
    {
        var storeEntity = await _dbContext.Stores
            .FirstOrDefaultAsync(s => s.Id == storeId);
            
        if (storeEntity == null)
            return false;
            
        storeEntity.Status = status;
        storeEntity.UpdatedAt = DateTime.UtcNow;
        
        _dbContext.Stores.Update(storeEntity);
        await _dbContext.SaveChangesAsync();
        
        return true;
    }

    public async Task<bool> AddSellerToStoreAsync(Guid storeId, Guid sellerId)
    {
        // Check if store and seller exist
        var storeExists = await _dbContext.Stores.AnyAsync(s => s.Id == storeId);
        var sellerExists = await _dbContext.Users.AnyAsync(u => u.Id == sellerId && u.Role == UserRole.Seller);
        
        if (!storeExists || !sellerExists)
            return false;
            
        // Check if assignment already exists
        var assignmentExists = await _dbContext.StoreSellers
            .AnyAsync(ss => ss.StoreId == storeId && ss.UserId == sellerId);
            
        if (assignmentExists)
            return true; // Already assigned
            
        // Create new assignment
        var assignment = new StoreSellerEntity
        {
            StoreId = storeId,
            UserId = sellerId,
            AssignedAt = DateTime.UtcNow
        };
        
        await _dbContext.StoreSellers.AddAsync(assignment);
        await _dbContext.SaveChangesAsync();
        
        return true;
    }

    public async Task<bool> RemoveSellerFromStoreAsync(Guid storeId, Guid sellerId)
    {
        var assignment = await _dbContext.StoreSellers
            .FirstOrDefaultAsync(ss => ss.StoreId == storeId && ss.UserId == sellerId);
            
        if (assignment == null)
            return false;
            
        _dbContext.StoreSellers.Remove(assignment);
        await _dbContext.SaveChangesAsync();
        
        return true;
    }

    public async Task<IEnumerable<UserDto>> GetSellersByStoreIdAsync(Guid storeId)
    {
        var sellers = await _dbContext.StoreSellers
            .AsNoTracking()
            .Where(ss => ss.StoreId == storeId)
            .Join(_dbContext.Users,
                ss => ss.UserId,
                u => u.Id,
                (ss, u) => u)
            .Where(u => u.Role == UserRole.Seller)
            .ToListAsync();
            
        return sellers.Select(s => new UserDto
        {
            Id = s.Id,
            Username = s.Username,
            Email = s.Email,
            Role = s.Role,
            BonusBalance = s.BonusBalance
        });
    }

    public async Task<IEnumerable<StoreDto>> GetStoresByCategoryAsync(string category)
    {
        var stores = await _dbContext.Stores
            .AsNoTracking()
            .Where(s => s.Location == category) // Assuming Location is used as category
            .OrderBy(s => s.Name)
            .ToListAsync();
            
        return stores.Select(MapToDto);
    }

    public async Task<decimal> GetStoreBonusBalanceAsync(Guid storeId)
    {
        // Calculate store bonus balance from transactions
        var totalBonus = await _dbContext.Transactions
            .AsNoTracking()
            .Where(t => t.StoreId == storeId)
            .SumAsync(t => t.Amount);
            
        return totalBonus;
    }
    
    // Helper method to map entity to DTO
    private static StoreDto MapToDto(StoreEntity entity)
    {
        return new StoreDto
        {
            Id = entity.Id,
            CompanyId = entity.CompanyId,
            Name = entity.Name,
            Location = entity.Location,
            Address = entity.Address,
            ContactPhone = entity.ContactPhone,
            Status = entity.Status
        };
    }
}