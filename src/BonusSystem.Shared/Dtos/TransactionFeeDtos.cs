namespace BonusSystem.Shared.Dtos;

public record TransactionFeeRequest
{
    public decimal FeePercent { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? EndDate { get; init; }
}
public record CompanyFeeResult
{
    public Guid CompanyId { get; init; }
    public int TotalTransactions { get; init; }
    public decimal TotalFee { get; init; }
}