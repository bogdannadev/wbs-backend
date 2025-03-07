using BonusSystem.Shared.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BonusSystem.Infrastructure.DataAccess.Postgres.Entities;

public sealed class TransactionEntity
{
    [Key]
    public Guid Id { get; set; }
    
    public Guid? UserId { get; set; }
    
    public Guid? CompanyId { get; set; }
    
    public Guid? StoreId { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }
    
    public TransactionType Type { get; set; }
    
    public DateTime Timestamp { get; set; }
    
    public TransactionStatus Status { get; set; }
    
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    // Navigation properties
    [ForeignKey(nameof(UserId))]
    public UserEntity? User { get; set; }
    
    [ForeignKey(nameof(CompanyId))]
    public CompanyEntity? Company { get; set; }
    
    [ForeignKey(nameof(StoreId))]
    public StoreEntity? Store { get; set; }
}