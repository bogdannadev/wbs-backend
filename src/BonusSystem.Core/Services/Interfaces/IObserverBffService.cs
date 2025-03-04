using BonusSystem.Shared.Dtos;

namespace BonusSystem.Core.Services.Interfaces;

public record StatisticsQueryDto
{
    public Guid? CompanyId { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
}

public record DashboardStatisticsDto
{
    public decimal TotalBonusCirculation { get; init; }
    public decimal CurrentActiveBonus { get; init; }
    public int TotalTransactions { get; init; }
    public int ActiveUsers { get; init; }
    public int ActiveCompanies { get; init; }
    public int ActiveStores { get; init; }
}

public interface IObserverBffService : IBaseBffService
{
    Task<DashboardStatisticsDto> GetStatisticsAsync(StatisticsQueryDto query);
    Task<TransactionDto> GetTransactionSummaryAsync(Guid? companyId);
    Task<IEnumerable<CompanySummaryDto>> GetCompaniesOverviewAsync();
}