using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BonusSystem.Infrastructure.DataAccess.Postgres.Entities;

public class StoreSellerEntity
{
    [Key]
    public Guid Id { get; set; }
    
    public Guid StoreId { get; set; }
    
    public Guid UserId { get; set; }
    
    public DateTime AssignedAt { get; set; }

    // Navigation properties
    [ForeignKey(nameof(StoreId))]
    public virtual StoreEntity Store { get; set; } = null!;
    
    [ForeignKey(nameof(UserId))]
    public virtual UserEntity User { get; set; } = null!;
}