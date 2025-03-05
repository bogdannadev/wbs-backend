using BonusSystem.Core.Repositories;
using BonusSystem.Infrastructure.DataAccess.Postgres.Entities;
using BonusSystem.Shared.Dtos;
using BonusSystem.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BonusSystem.Infrastructure.DataAccess.Postgres.Repositories;

public class PostgresUserRepository : IUserRepository
{
    private readonly BonusSystemDbContext _dbContext;
    private readonly ILogger<PostgresUserRepository> _logger;

    public PostgresUserRepository(BonusSystemDbContext dbContext, ILogger<PostgresUserRepository> logger)
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

    public async Task<bool> UpdateBalanceAsync(Guid userId, decimal newBalance)
    {
        try
        {
            var entity = await _dbContext.Users.FindAsync(userId);
            if (entity == null)
            {
                return false;
            }

            entity.BonusBalance = newBalance;
            await _dbContext.SaveChangesAsync();
            
            return true;
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

    private UserDto MapToDto(UserEntity entity)
    {
        return new UserDto
        {
            Id = entity.Id,
            Username = entity.Username,
            Email = entity.Email,
            Role = entity.Role,
            BonusBalance = entity.BonusBalance
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
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }
}