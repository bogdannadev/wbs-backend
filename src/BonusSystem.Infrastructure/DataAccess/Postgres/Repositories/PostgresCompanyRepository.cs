using BonusSystem.Core.Repositories;
using BonusSystem.Shared.Dtos;
using BonusSystem.Shared.Models;

namespace BonusSystem.Infrastructure.DataAccess.Postgres.Repositories;

public class PostgresCompanyRepository : ICompanyRepository
{
    public Task<IEnumerable<CompanyDto>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<CompanyDto?> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Guid> CreateAsync(CompanyDto entity)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateAsync(CompanyDto entity)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateBalanceAsync(Guid companyId, decimal newBalance)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateStatusAsync(Guid companyId, CompanyStatus status)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<CompanyDto>> GetCompaniesByStatusAsync(CompanyStatus status)
    {
        throw new NotImplementedException();
    }

    public Task<decimal> GetOriginalBalanceAsync(Guid companyId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ResetToOriginalBalanceAsync(Guid companyId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<CompanySummaryDto>> GetCompanySummariesAsync()
    {
        throw new NotImplementedException();
    }
}