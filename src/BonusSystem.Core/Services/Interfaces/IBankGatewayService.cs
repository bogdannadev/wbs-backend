namespace BonusSystem.Core.Services.Interfaces; 
public interface IBankGatewayService
{
    Task<string> CreatePaymentSessionAsync(decimal amount, string userId, string callbackUrl);
    
}
