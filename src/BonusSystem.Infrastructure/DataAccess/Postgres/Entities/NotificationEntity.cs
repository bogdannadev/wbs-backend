using BonusSystem.Shared.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BonusSystem.Infrastructure.DataAccess.Postgres.Entities;

public class NotificationEntity
{
    [Key]
    public Guid Id { get; set; }
    
    public Guid RecipientId { get; set; }
    
    [Required, MaxLength(500)]
    public string Message { get; set; } = string.Empty;
    
    public NotificationType Type { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public bool IsRead { get; set; }

    // Navigation properties
    [ForeignKey(nameof(RecipientId))]
    public virtual UserEntity Recipient { get; set; } = null!;
}