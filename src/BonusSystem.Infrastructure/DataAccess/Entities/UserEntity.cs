using BonusSystem.Shared.Models;

namespace BonusSystem.Infrastructure.DataAccess.Entities;

public class UserEntity
{
    public Guid Id { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public UserRole Role { get; set; }
    public decimal BonusBalance { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLogin { get; set; }
    public bool IsActive { get; set; }
    
    // Navigation properties
    public ICollection<BonusTransactionEntity> Transactions { get; set; } = new List<BonusTransactionEntity>();
    public ICollection<NotificationEntity> Notifications { get; set; } = new List<NotificationEntity>();
    public ICollection<StoreSellerAssignmentEntity> StoreAssignments { get; set; } = new List<StoreSellerAssignmentEntity>();
}
