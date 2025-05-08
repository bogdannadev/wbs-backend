using BonusSystem.Shared.Models;

namespace BonusSystem.Shared.Dtos;

public record TransactionDto
{
    public Guid Id { get; init; }
    public Guid? UserId { get; init; }
    public Guid? CompanyId { get; init; }
    public Guid? StoreId { get; init; }
    public decimal BonusAmount { get; init; }
    public decimal TotalCost { get; init; } 
    public decimal CashbackAmount { get; init; }
    public TransactionType Type { get; init; }
    public DateTime Timestamp { get; init; }
    public TransactionStatus Status { get; init; }
    public string Description { get; init; } = string.Empty;
} 

public record FiatTransactionDto
{ 
    public Guid Id { get; init; }
    public Guid? BuyerId { get; init; }
    public decimal BonusAmount { get; init; }
    public TransactionType Type { get; init; }
    public DateTime Timestamp { get; init; }
    public TransactionStatus Status { get; init; }
    public string Description { get; init; } = string.Empty;  
}

public record TransactionRequestDto
{
    public Guid BuyerId { get; init; }
    public decimal BonusAmount { get; init; }
    public decimal TotalCost { get; set; }
    public TransactionType Type { get; init; }
    
}

public record TransactionResultDto
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
    public TransactionDto? Transaction { get; init; }
}

public record BonusTransactionSummaryDto
{
    public decimal TotalEarned { get; init; }
    public decimal TotalSpent { get; init; }
    public decimal CurrentBalance { get; init; }
    public decimal ExpiringNextQuarter { get; init; }
    public List<TransactionDto> RecentTransactions { get; init; } = new();
}

public record StoreBonusTransactionsDto
{
    public Guid StoreId { get; init; }
    public string StoreName { get; init; } = string.Empty;
    public decimal TotalTransactions { get; init; }
    public List<TransactionDto> Transactions { get; init; } = new();
} 
public record ReplenishmentBonusBalance
{
    public Guid BuyerId { get; init; }
    public decimal Amount { get; init; }
    public DateTime Timestamp { get; init; }
    public bool IsValid()
    {
        return Amount > 0;
    }
} 

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
    public string TransactionId { get; set; }
    public string ErrorMessage { get; set; }
}