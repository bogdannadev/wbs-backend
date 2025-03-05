using BonusSystem.Shared.Models;

namespace BonusSystem.Infrastructure.DataAccess.Entities;

public class StoreEntity
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public required string Name { get; set; }
    public string? Location { get; set; }
    public string? Address { get; set; }
    public string? ContactPhone { get; set; }
    public string? Category { get; set; }
    public StoreStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public CompanyEntity Company { get; set; } = null!;
    public ICollection<BonusTransactionEntity> Transactions { get; set; } = new List<BonusTransactionEntity>();
    public ICollection<StoreSellerAssignmentEntity> SellerAssignments { get; set; } = new List<StoreSellerAssignmentEntity>();
}
