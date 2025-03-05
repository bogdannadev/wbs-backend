using BonusSystem.Shared.Models;

namespace BonusSystem.Infrastructure.DataAccess.Entities;

public class BonusTransactionEntity
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public Guid? CompanyId { get; set; }
    public Guid? StoreId { get; set; }
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
    public DateTime Timestamp { get; set; }
    public TransactionStatus Status { get; set; }
    public string? Description { get; set; }
    
    // Navigation properties
    public UserEntity? User { get; set; }
    public CompanyEntity? Company { get; set; }
    public StoreEntity? Store { get; set; }
}
