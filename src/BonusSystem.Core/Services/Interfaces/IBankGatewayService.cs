using BonusSystem.Shared.Dtos;

namespace BonusSystem.Core.Services.Interfaces; 
public interface IBankGatewayService
{
    Task<PaymentResult> ProcessPaymentAsync(PaymentRequest paymentRequest);
}
