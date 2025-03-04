using BonusSystem.Shared.Dtos;

namespace BonusSystem.Core.Services.Interfaces;

public interface ISellerBffService : IBaseBffService
{
    Task<TransactionResultDto> ProcessTransactionAsync(Guid sellerId, TransactionRequestDto request);
    Task<bool> ConfirmTransactionReturnAsync(Guid sellerId, Guid transactionId);
    Task<decimal> GetBuyerBonusBalanceAsync(Guid buyerId);
    Task<decimal> GetStoreBonusBalanceAsync(Guid storeId);
    Task<IEnumerable<StoreBonusTransactionsDto>> GetStoreBonusTransactionsAsync(Guid storeId);
}