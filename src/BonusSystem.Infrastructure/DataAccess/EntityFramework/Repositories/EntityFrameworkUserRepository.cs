using BonusSystem.Core.Exceptions;
using BonusSystem.Core.Repositories;
using BonusSystem.Infrastructure.DataAccess.Entities;
using BonusSystem.Shared.Dtos;
using BonusSystem.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BonusSystem.Infrastructure.DataAccess.EntityFramework.Repositories;

public class EntityFrameworkUserRepository : IUserRepository
{
    private readonly BonusSystemContext _dbContext;
    private readonly ILogger<EntityFrameworkUserRepository> _logger;

    public EntityFrameworkUserRepository(BonusSystemContext dbContext, ILogger<EntityFrameworkUserRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        try
        {
            var entities = await _dbContext.Users.AsNoTracking().ToListAsync();
            return entities.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all users");
            throw;
        }
    }

    public async Task<UserDto?> GetByIdAsync(Guid id)
    {
        try
        {
            var entity = await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
            return entity != null ? MapToDto(entity) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user with ID {Id}", id);
            throw;
        }
    }

    public async Task<Guid> CreateAsync(UserDto dto)
    {
        try
        {
            var entity = MapToEntity(dto);
            entity.CreatedAt = DateTime.UtcNow;
            
            await _dbContext.Users.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            
            return entity.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user {Username}", dto.Username);
            throw;
        }
    }

    public async Task<bool> UpdateAsync(UserDto dto)
    {
        try
        {
            var entity = await _dbContext.Users.FindAsync(dto.Id);
            if (entity == null)
            {
                return false;
            }

            entity.Username = dto.Username;
            entity.Email = dto.Email;
            entity.Role = dto.Role;
            entity.BonusBalance = dto.BonusBalance;
            entity.CompanyId = dto.CompanyId;
            
            await _dbContext.SaveChangesAsync();
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user with ID {Id}", dto.Id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        try
        {
            var entity = await _dbContext.Users.FindAsync(id);
            if (entity == null)
            {
                return false;
            }

            _dbContext.Users.Remove(entity);
            await _dbContext.SaveChangesAsync();
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user with ID {Id}", id);
            throw;
        }
    }

    public async Task<UserDto?> GetByEmailAsync(string email)
    {
        try
        {
            var entity = await _dbContext.Users.AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
            
            return entity != null ? MapToDto(entity) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user with email {Email}", email);
            throw;
        }
    }

    public async Task<bool> UpdateBalanceAsync(Guid userId, decimal newBalance, decimal expectedCurrentBalance)
    {
        try
        {
            var result = await _dbContext.Database.ExecuteSqlInterpolatedAsync($@"
                UPDATE users
                SET ""BonusBalance"" = {newBalance}
                WHERE ""Id"" = {userId} AND ""BonusBalance"" = {expectedCurrentBalance}");

            if (result == 0)
                throw new ConcurrencyException($"User {userId} balance update conflict — expected value was {expectedCurrentBalance}");
                
            return result > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating balance for user with ID {Id}", userId);
            throw;
        }
    }

    public async Task<UserRole> GetUserRoleAsync(Guid userId)
    {
        try
        {
            var role = await _dbContext.Users.AsNoTracking()
                .Where(u => u.Id == userId)
                .Select(u => u.Role)
                .FirstOrDefaultAsync();
            
            return role;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving role for user with ID {Id}", userId);
            throw;
        }
    }

    public async Task<bool> IsUserExistsByEmailAsync(string email)
    {
        try
        {
            return await _dbContext.Users.AsNoTracking()
                .AnyAsync(u => u.Email.ToLower() == email.ToLower());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if user exists with email {Email}", email);
            throw;
        }
    }

    public async Task<IEnumerable<UserDto>> GetUsersByRoleAsync(UserRole role)
    {
        try
        {
            var entities = await _dbContext.Users.AsNoTracking()
                .Where(u => u.Role == role)
                .ToListAsync();
            
            return entities.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users with role {Role}", role);
            throw;
        }
    }

    public async Task<IEnumerable<UserDto>> GetUsersByCompanyIdAsync(Guid companyId)
    {
        try
        {
            var entities = await _dbContext.Users.AsNoTracking()
                .Where(u => u.CompanyId == companyId)
                .ToListAsync();
            
            return entities.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users for company with ID {CompanyId}", companyId);
            throw;
        }
    }

    public async Task<bool> AssignUserToCompanyAsync(Guid userId, Guid companyId)
    {
        try
        {
            // Check if the company exists
            var companyExists = await _dbContext.Companies.AnyAsync(c => c.Id == companyId);
            if (!companyExists)
            {
                _logger.LogWarning("Cannot assign user to non-existent company. Company ID: {CompanyId}", companyId);
                return false;
            }
            
            // Find the user
            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("Cannot assign non-existent user to company. User ID: {UserId}", userId);
                return false;
            }
            
            // Only allow assigning Company or Seller roles to a company
            if (user.Role != UserRole.Company && user.Role != UserRole.Seller)
            {
                _logger.LogWarning("Cannot assign user with role {Role} to company. Only Company and Seller roles can be assigned.", user.Role);
                return false;
            }
            
            // Assign company ID
            user.CompanyId = companyId;
            await _dbContext.SaveChangesAsync();
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning user {UserId} to company {CompanyId}", userId, companyId);
            throw;
        }
    }

    public async Task<UserDto?> GetCompanyAdminUserByCompanyIdAsync(Guid companyId)
    {
        try
        {
            // Find the company admin (user with Company role and the specified company ID)
            var entity = await _dbContext.Users.AsNoTracking()
                .Where(u => u.CompanyId == companyId && u.Role == UserRole.Company)
                .FirstOrDefaultAsync();
            
            return entity != null ? MapToDto(entity) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving admin user for company with ID {CompanyId}", companyId);
            throw;
        }
    }

    private UserDto MapToDto(UserEntity entity)
    {
        return new UserDto
        {
            Id = entity.Id,
            Username = entity.Username,
            Email = entity.Email,
            Role = entity.Role,
            BonusBalance = entity.BonusBalance,
            CompanyId = entity.CompanyId
        };
    }

    private UserEntity MapToEntity(UserDto dto)
    {
        return new UserEntity
        {
            Id = dto.Id,
            Username = dto.Username,
            Email = dto.Email,
            Role = dto.Role,
            BonusBalance = dto.BonusBalance,
            CompanyId = dto.CompanyId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }
}