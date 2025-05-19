using BonusSystem.Shared.Dtos;

namespace BonusSystem.Core.Services.Interfaces;

public interface IPaymentService
{
    Task<PaymentResult> CreatePaymentAsync(FiatTransactionDto transaction);
    Task<bool> HandleWebhookAsync(string json, string signature);
}