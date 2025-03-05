using BonusSystem.Core.Repositories;
using BonusSystem.Shared.Dtos;
using BonusSystem.Shared.Models;

namespace BonusSystem.Infrastructure.DataAccess.InMemory;

/// <summary>
/// In-memory implementation of the company repository
/// </summary>
public class InMemoryCompanyRepository : InMemoryRepository<CompanyDto, Guid>, ICompanyRepository
{
    public InMemoryCompanyRepository() : base(company => company.Id)
    {
        // Initialize with some sample data
        var companies = new List<CompanyDto>
        {
            new()
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                Name = "Alpha Store Chain",
                ContactEmail = "contact@alphastore.com",
                ContactPhone = "+1234567890",
                BonusBalance = 1000000,
                OriginalBonusBalance = 1000000,
                Status = CompanyStatus.Active,
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                Stores = new List<StoreDto>()
            },
            new()
            {
                Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                Name = "Beta Retail Group",
                ContactEmail = "info@betaretail.com",
                ContactPhone = "+1987654321",
                BonusBalance = 500000,
                OriginalBonusBalance = 500000,
                Status = CompanyStatus.Active,
                CreatedAt = DateTime.UtcNow.AddDays(-15),
                Stores = new List<StoreDto>()
            },
            new()
            {
                Id = Guid.Parse("66666666-6666-6666-6666-666666666666"),
                Name = "Gamma Markets",
                ContactEmail = "support@gammamarkets.com",
                ContactPhone = "+1122334455",
                BonusBalance = 250000,
                OriginalBonusBalance = 250000,
                Status = CompanyStatus.Pending,
                CreatedAt = DateTime.UtcNow.AddDays(-5),
                Stores = new List<StoreDto>()
            }
        };

        foreach (var company in companies)
        {
            _entities[company.Id] = company;
        }
    }

    public Task<bool> UpdateBalanceAsync(Guid companyId, decimal newBalance)
    {
        if (!_entities.TryGetValue(companyId, out var company))
        {
            return Task.FromResult(false);
        }

        var updatedCompany = company with { BonusBalance = newBalance };
        return Task.FromResult(_entities.TryUpdate(companyId, updatedCompany, company));
    }

    public Task<bool> UpdateStatusAsync(Guid companyId, CompanyStatus status)
    {
        if (!_entities.TryGetValue(companyId, out var company))
        {
            return Task.FromResult(false);
        }

        var updatedCompany = company with { Status = status };
        return Task.FromResult(_entities.TryUpdate(companyId, updatedCompany, company));
    }

    public Task<IEnumerable<CompanyDto>> GetCompaniesByStatusAsync(CompanyStatus status)
    {
        var companies = _entities.Values.Where(c => c.Status == status);
        return Task.FromResult(companies);
    }

    public Task<decimal> GetOriginalBalanceAsync(Guid companyId)
    {
        if (!_entities.TryGetValue(companyId, out var company))
        {
            return Task.FromResult(0m);
        }

        return Task.FromResult(company.OriginalBonusBalance);
    }

    public Task<bool> ResetToOriginalBalanceAsync(Guid companyId)
    {
        if (!_entities.TryGetValue(companyId, out var company))
        {
            return Task.FromResult(false);
        }

        var updatedCompany = company with { BonusBalance = company.OriginalBonusBalance };
        return Task.FromResult(_entities.TryUpdate(companyId, updatedCompany, company));
    }

    public Task<IEnumerable<CompanySummaryDto>> GetCompanySummariesAsync()
    {
        var summaries = _entities.Values.Select(c => new CompanySummaryDto
        {
            Id = c.Id,
            Name = c.Name,
            BonusBalance = c.BonusBalance,
            TransactionVolume = 0, // Would be calculated from transactions in a real implementation
            StoreCount = c.Stores.Count,
            Status = c.Status
        });

        return Task.FromResult(summaries);
    }
}