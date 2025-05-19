namespace BonusSystem.Shared.Dtos;

public class FiatTransactionDto
{
    public Guid BuyerId { get; set; }
    public PaymentBodyDto PaymentBody { get; set; }
    public string Description { get; set; }
}

public class PaymentBodyDto
{
    public decimal Amount { get; set; }
    public string Currency { get; set; }
}

public class PaymentResult
{
    public bool Success { get; set; }
    public string ClientSecret { get; set; }
    public string ErrorMessage { get; set; }
}