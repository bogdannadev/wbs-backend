using BonusSystem.Shared.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BonusSystem.Infrastructure.DataAccess.Postgres.Entities;

public class CompanyEntity
{
    [Key]
    public Guid Id { get; set; }
    
    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(255)]
    public string ContactEmail { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string ContactPhone { get; set; } = string.Empty;
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal BonusBalance { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal OriginalBonusBalance { get; set; }
    
    public CompanyStatus Status { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public virtual ICollection<StoreEntity> Stores { get; set; } = new List<StoreEntity>();
    public virtual ICollection<TransactionEntity> Transactions { get; set; } = new List<TransactionEntity>();
}