using BonusSystem.Shared.Dtos;
using BonusSystem.Shared.Models;

namespace BonusSystem.Core.Repositories;

/// <summary>
/// Repository for company-related operations
/// </summary>
public interface ICompanyRepository : IRepository<CompanyDto, Guid>
{
    Task<bool> UpdateBalanceAsync(Guid companyId, decimal newBalance);
    Task<bool> UpdateStatusAsync(Guid companyId, CompanyStatus status);
    Task<IEnumerable<CompanyDto>> GetCompaniesByStatusAsync(CompanyStatus status);
    Task<decimal> GetOriginalBalanceAsync(Guid companyId);
    Task<bool> ResetToOriginalBalanceAsync(Guid companyId);
    Task<IEnumerable<CompanySummaryDto>> GetCompanySummariesAsync();
}