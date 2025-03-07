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
        try
        {
            var entities = await _dbContext.Stores.AsNoTracking().ToListAsync();
            return entities.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all stores");
            throw;
        }
    }

    public async Task<StoreDto?> GetByIdAsync(Guid id)
    {
        try
        {
            var entity = await _dbContext.Stores.AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);
            return entity != null ? MapToDto(entity) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving store with ID {Id}", id);
            throw;
        }
    }

    public async Task<Guid> CreateAsync(StoreDto dto)
    {
        try
        {
            var entity = MapToEntity(dto);
            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;
            
            await _dbContext.Stores.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            
            return entity.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating store {Name}", dto.Name);
            throw;
        }
    }

    public async Task<bool> UpdateAsync(StoreDto dto)
    {
        try
        {
            var entity = await _dbContext.Stores.FindAsync(dto.Id);
            if (entity == null)
            {
                return false;
            }

            entity.Name = dto.Name;
            entity.Location = dto.Location;
            entity.Address = dto.Address;
            entity.ContactPhone = dto.ContactPhone;
            entity.Status = dto.Status;
            entity.UpdatedAt = DateTime.UtcNow;
            
            await _dbContext.SaveChangesAsync();
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating store with ID {Id}", dto.Id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        try
        {
            var entity = await _dbContext.Stores.FindAsync(id);
            if (entity == null)
            {
                return false;
            }

            _dbContext.Stores.Remove(entity);
            await _dbContext.SaveChangesAsync();
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting store with ID {Id}", id);
            throw;
        }
    }

    public async Task<IEnumerable<StoreDto>> GetStoresByCompanyIdAsync(Guid companyId)
    {
        try
        {
            var entities = await _dbContext.Stores.AsNoTracking()
                .Where(s => s.CompanyId == companyId)
                .ToListAsync();
            
            return entities.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving stores for company ID {CompanyId}", companyId);
            throw;
        }
    }

    public async Task<bool> UpdateStatusAsync(Guid storeId, StoreStatus status)
    {
        try
        {
            var entity = await _dbContext.Stores.FindAsync(storeId);
            if (entity == null)
            {
                return false;
            }

            entity.Status = status;
            entity.UpdatedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating status for store with ID {Id}", storeId);
            throw;
        }
    }

    public async Task<bool> AddSellerToStoreAsync(Guid storeId, Guid sellerId)
    {
        try
        {
            // First check if this assignment already exists
            var exists = await _dbContext.StoreSellers.AnyAsync(
                ss => ss.StoreId == storeId && ss.UserId == sellerId);
            
            if (exists)
            {
                return true; // Already assigned
            }

            // Create new assignment
            var assignment = new StoreSellerEntity
            {
                Id = Guid.NewGuid(),
                StoreId = storeId,
                UserId = sellerId,
                AssignedAt = DateTime.UtcNow
            };
            
            await _dbContext.StoreSellers.AddAsync(assignment);
            await _dbContext.SaveChangesAsync();
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding seller {SellerId} to store {StoreId}", sellerId, storeId);
            throw;
        }
    }

    public async Task<bool> RemoveSellerFromStoreAsync(Guid storeId, Guid sellerId)
    {
        try
        {
            var assignment = await _dbContext.StoreSellers
                .FirstOrDefaultAsync(ss => ss.StoreId == storeId && ss.UserId == sellerId);
            
            if (assignment == null)
            {
                return false; // No such assignment exists
            }

            _dbContext.StoreSellers.Remove(assignment);
            await _dbContext.SaveChangesAsync();
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing seller {SellerId} from store {StoreId}", sellerId, storeId);
            throw;
        }
    }

    public async Task<IEnumerable<UserDto>> GetSellersByStoreIdAsync(Guid storeId)
    {
        try
        {
            var sellerIds = await _dbContext.StoreSellers
                .Where(ss => ss.StoreId == storeId)
                .Select(ss => ss.UserId)
                .ToListAsync();

            var sellers = await _dbContext.Users
                .Where(u => sellerIds.Contains(u.Id))
                .ToListAsync();
            
            return sellers.Select(seller => new UserDto
            {
                Id = seller.Id,
                Username = seller.Username,
                Email = seller.Email,
                Role = seller.Role,
                BonusBalance = seller.BonusBalance
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving sellers for store ID {StoreId}", storeId);
            throw;
        }
    }

    public async Task<IEnumerable<StoreDto>> GetStoresByCategoryAsync(string category)
    {
        try
        {
            // For a proper implementation, you would need a Categories table and relationships
            // For this prototype, we're just doing a simple search by name/location
            var entities = await _dbContext.Stores.AsNoTracking()
                .Where(s => s.Name.Contains(category) || s.Location.Contains(category))
                .ToListAsync();
            
            return entities.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving stores by category {Category}", category);
            throw;
        }
    }

    public async Task<decimal> GetStoreBonusBalanceAsync(Guid storeId)
    {
        try
        {
            // This is a bit of a complex calculation - in a real implementation,
            // you might have a different approach, but for the prototype:
            // Sum all completed transactions for this store
            var transactionsSum = await _dbContext.Transactions
                .Where(t => t.StoreId == storeId && t.Status == TransactionStatus.Completed)
                .SumAsync(t => t.Type == TransactionType.Earn ? t.Amount : -t.Amount);
            
            return transactionsSum;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating bonus balance for store with ID {Id}", storeId);
            throw;
        }
    }

    private StoreDto MapToDto(StoreEntity entity)
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

    private StoreEntity MapToEntity(StoreDto dto)
    {
        return new StoreEntity
        {
            Id = dto.Id,
            CompanyId = dto.CompanyId,
            Name = dto.Name,
            Location = dto.Location,
            Address = dto.Address,
            ContactPhone = dto.ContactPhone,
            Status = dto.Status,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }
}