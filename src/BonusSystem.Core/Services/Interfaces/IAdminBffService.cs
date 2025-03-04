using BonusSystem.Shared.Dtos;
using BonusSystem.Shared.Models;

namespace BonusSystem.Core.Services.Interfaces;

public interface IAdminBffService : IBaseBffService
{
    Task<CompanyRegistrationResultDto> RegisterCompanyAsync(CompanyRegistrationDto request);
    Task<bool> UpdateCompanyStatusAsync(Guid companyId, CompanyStatus status);
    Task<bool> ModerateStoreAsync(Guid storeId, bool isApproved);
    Task<bool> CreditCompanyBalanceAsync(Guid companyId, decimal amount);
    Task<IEnumerable<TransactionDto>> GetSystemTransactionsAsync(Guid? companyId = null, DateTime? startDate = null, DateTime? endDate = null);
    Task<bool> SendSystemNotificationAsync(Guid? recipientId, string message, NotificationType type);
}