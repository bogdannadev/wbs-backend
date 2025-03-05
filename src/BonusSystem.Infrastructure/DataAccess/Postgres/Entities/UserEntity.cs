using BonusSystem.Shared.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BonusSystem.Infrastructure.DataAccess.Postgres.Entities;

public class UserEntity
{
    [Key]
    public Guid Id { get; set; }
    
    [Required, MaxLength(100)]
    public string Username { get; set; } = string.Empty;
    
    [Required, MaxLength(255)]
    public string Email { get; set; } = string.Empty;
    
    public UserRole Role { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal BonusBalance { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime? LastLogin { get; set; }
    
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public virtual ICollection<TransactionEntity> Transactions { get; set; } = new List<TransactionEntity>();
    public virtual ICollection<NotificationEntity> Notifications { get; set; } = new List<NotificationEntity>();
    public virtual ICollection<StoreSellerEntity> StoreAssignments { get; set; } = new List<StoreSellerEntity>();
}