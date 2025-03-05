using BonusSystem.Shared.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BonusSystem.Infrastructure.DataAccess.Postgres.Entities;

public class StoreEntity
{
    [Key]
    public Guid Id { get; set; }
    
    public Guid CompanyId { get; set; }
    
    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string Location { get; set; } = string.Empty;
    
    [MaxLength(255)]
    public string Address { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string ContactPhone { get; set; } = string.Empty;
    
    public StoreStatus Status { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    [ForeignKey(nameof(CompanyId))]
    public virtual CompanyEntity Company { get; set; } = null!;
    public virtual ICollection<TransactionEntity> Transactions { get; set; } = new List<TransactionEntity>();
    public virtual ICollection<StoreSellerEntity> SellerAssignments { get; set; } = new List<StoreSellerEntity>();
}