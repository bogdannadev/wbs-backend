namespace BonusSystem.Infrastructure.DataAccess.Entities;

public class StoreSellerAssignmentEntity
{
    public Guid Id { get; set; }
    public Guid StoreId { get; set; }
    public Guid UserId { get; set; }
    public DateTime AssignedAt { get; set; }
    
    // Navigation properties
    public StoreEntity Store { get; set; } = null!;
    public UserEntity User { get; set; } = null!;
}
