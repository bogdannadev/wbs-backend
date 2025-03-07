using BonusSystem.Core.Repositories;
using BonusSystem.Infrastructure.DataAccess.Postgres.Entities;
using BonusSystem.Shared.Dtos;
using BonusSystem.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BonusSystem.Infrastructure.DataAccess.Postgres.Repositories;

public class PostgresCompanyRepository : ICompanyRepository
{
    private readonly BonusSystemDbContext _dbContext;
    private readonly ILogger<PostgresCompanyRepository> _logger;

    public PostgresCompanyRepository(BonusSystemDbContext dbContext, ILogger<PostgresCompanyRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<IEnumerable<CompanyDto>> GetAllAsync()
    {
        try
        {
            var entities = await _dbContext.Companies.AsNoTracking().ToListAsync();
            return entities.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all companies");
            throw;
        }
    }

    public async Task<CompanyDto?> GetByIdAsync(Guid id)
    {
        try
        {
            var entity = await _dbContext.Companies.AsNoTracking()
                .Include(c => c.Stores)
                .FirstOrDefaultAsync(c => c.Id == id);
            return entity != null ? MapToDto(entity) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving company with ID {Id}", id);
            throw;
        }
    }

    public async Task<Guid> CreateAsync(CompanyDto dto)
    {
        try
        {
            var entity = MapToEntity(dto);
            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;
            
            await _dbContext.Companies.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            
            return entity.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating company {Name}", dto.Name);
            throw;
        }
    }

    public async Task<bool> UpdateAsync(CompanyDto dto)
    {
        try
        {
            var entity = await _dbContext.Companies.FindAsync(dto.Id);
            if (entity == null)
            {
                return false;
            }

            entity.Name = dto.Name;
            entity.ContactEmail = dto.ContactEmail;
            entity.ContactPhone = dto.ContactPhone;
            entity.BonusBalance = dto.BonusBalance;
            entity.Status = dto.Status;
            entity.UpdatedAt = DateTime.UtcNow;
            
            await _dbContext.SaveChangesAsync();
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating company with ID {Id}", dto.Id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        try
        {
            var entity = await _dbContext.Companies.FindAsync(id);
            if (entity == null)
            {
                return false;
            }

            _dbContext.Companies.Remove(entity);
            await _dbContext.SaveChangesAsync();
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting company with ID {Id}", id);
            throw;
        }
    }

    public async Task<bool> UpdateBalanceAsync(Guid companyId, decimal newBalance)
    {
        try
        {
            var entity = await _dbContext.Companies.FindAsync(companyId);
            if (entity == null)
            {
                return false;
            }

            entity.BonusBalance = newBalance;
            entity.UpdatedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating balance for company with ID {Id}", companyId);
            throw;
        }
    }

    public async Task<bool> UpdateStatusAsync(Guid companyId, CompanyStatus status)
    {
        try
        {
            var entity = await _dbContext.Companies.FindAsync(companyId);
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
            _logger.LogError(ex, "Error updating status for company with ID {Id}", companyId);
            throw;
        }
    }

    public async Task<IEnumerable<CompanyDto>> GetCompaniesByStatusAsync(CompanyStatus status)
    {
        try
        {
            var entities = await _dbContext.Companies.AsNoTracking()
                .Where(c => c.Status == status)
                .ToListAsync();
            
            return entities.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving companies with status {Status}", status);
            throw;
        }
    }

    public async Task<decimal> GetOriginalBalanceAsync(Guid companyId)
    {
        try
        {
            var originalBalance = await _dbContext.Companies.AsNoTracking()
                .Where(c => c.Id == companyId)
                .Select(c => c.OriginalBonusBalance)
                .FirstOrDefaultAsync();
            
            return originalBalance;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving original balance for company with ID {Id}", companyId);
            throw;
        }
    }

    public async Task<bool> ResetToOriginalBalanceAsync(Guid companyId)
    {
        try
        {
            var entity = await _dbContext.Companies.FindAsync(companyId);
            if (entity == null)
            {
                return false;
            }

            entity.BonusBalance = entity.OriginalBonusBalance;
            entity.UpdatedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting balance for company with ID {Id}", companyId);
            throw;
        }
    }

    public async Task<IEnumerable<CompanySummaryDto>> GetCompanySummariesAsync()
    {
        try
        {
            var companies = await _dbContext.Companies.AsNoTracking()
                .Include(c => c.Stores)
                .ToListAsync();
            
            var summaries = new List<CompanySummaryDto>();
            
            foreach (var company in companies)
            {
                // Get transaction volume - this is a more complex calculation
                var transactionVolume = await _dbContext.Transactions
                    .Where(t => t.CompanyId == company.Id && t.Status == TransactionStatus.Completed)
                    .SumAsync(t => t.Amount);
                
                summaries.Add(new CompanySummaryDto
                {
                    Id = company.Id,
                    Name = company.Name,
                    BonusBalance = company.BonusBalance,
                    TransactionVolume = transactionVolume,
                    StoreCount = company.Stores.Count,
                    Status = company.Status
                });
            }
            
            return summaries;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving company summaries");
            throw;
        }
    }

    private CompanyDto MapToDto(CompanyEntity entity)
    {
        return new CompanyDto
        {
            Id = entity.Id,
            Name = entity.Name,
            ContactEmail = entity.ContactEmail,
            ContactPhone = entity.ContactPhone,
            BonusBalance = entity.BonusBalance,
            OriginalBonusBalance = entity.OriginalBonusBalance,
            Status = entity.Status,
            CreatedAt = entity.CreatedAt,
            Stores = entity.Stores?.Select(s => new StoreDto
            {
                Id = s.Id,
                CompanyId = s.CompanyId,
                Name = s.Name,
                Location = s.Location,
                Address = s.Address,
                ContactPhone = s.ContactPhone,
                Status = s.Status
            }).ToList() ?? new List<StoreDto>()
        };
    }

    private CompanyEntity MapToEntity(CompanyDto dto)
    {
        return new CompanyEntity
        {
            Id = dto.Id,
            Name = dto.Name,
            ContactEmail = dto.ContactEmail,
            ContactPhone = dto.ContactPhone,
            BonusBalance = dto.BonusBalance,
            OriginalBonusBalance = dto.OriginalBonusBalance,
            Status = dto.Status,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = DateTime.UtcNow
        };
    }
}