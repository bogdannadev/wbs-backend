using BonusSystem.Shared.Dtos;

namespace BonusSystem.Core.Services.Interfaces;

public interface ICompanyBffService : IBaseBffService
{
    Task<bool> RegisterStore(StoreRegistrationDto storeDto);
    Task<bool> RegisterSeller(UserRegistrationDto seller);
    Task<bool> RegisterSellerForCompany(SellerRegistrationDto seller, Guid companyId);
    Task<DashboardStatisticsDto> GetStatisticsAsync(StatisticsQueryDto query);
    Task<TransactionDto> GetTransactionSummaryAsync(Guid? companyId);
    Task<StoresWithSellersPagedResponseDto> GetStoresWithSellersAsync(Guid companyId, StoresFilterRequestDto filter);
}