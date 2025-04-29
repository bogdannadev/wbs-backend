using BonusSystem.Shared.Models;

namespace BonusSystem.Infrastructure.DataAccess.Entities;

public class CompanyEntity
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public decimal BonusBalance { get; set; }
    public decimal OriginalBonusBalance { get; set; }
    public CompanyStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public ICollection<StoreEntity> Stores { get; set; } = new List<StoreEntity>();
    public ICollection<BonusTransactionEntity> Transactions { get; set; } = new List<BonusTransactionEntity>();
    public ICollection<UserEntity> Users { get; set; } = new List<UserEntity>();
}
