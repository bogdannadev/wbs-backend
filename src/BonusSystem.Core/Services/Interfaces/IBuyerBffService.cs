using BonusSystem.Shared.Dtos;

namespace BonusSystem.Core.Services.Interfaces;

public interface IBuyerBffService : IBaseBffService
{
    Task<BonusTransactionSummaryDto> GetBonusSummaryAsync(Guid userId);
    Task<IEnumerable<TransactionDto>> GetTransactionHistoryAsync(Guid userId);
    Task<string> GenerateQrCodeAsync(Guid userId);
    Task<bool> CancelTransactionAsync(Guid userId, Guid transactionId, bool confirm);
    Task<IEnumerable<StoreDto>> FindStoresByCategoryAsync(string category);
}
