
namespace BonusSystem.Shared.Dtos; 

public class PaymentRequest
{
    public string CardNumber { get; set; }
    public string ExpiryDate { get; set; }
    public string Cvv { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; }
}

public class PaymentResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}
