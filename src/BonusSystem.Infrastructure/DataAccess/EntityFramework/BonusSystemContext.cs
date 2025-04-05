using BonusSystem.Infrastructure.DataAccess.Entities;
using BonusSystem.Infrastructure.DataAccess.EntityFramework.Configurations;
using Microsoft.EntityFrameworkCore;

namespace BonusSystem.Infrastructure.DataAccess.EntityFramework;

public sealed class BonusSystemContext : DbContext
{
    public BonusSystemContext(DbContextOptions<BonusSystemContext> options) : base(options)
    {
        Database.EnsureCreated();
    }
    
    public DbSet<UserEntity> Users { get; set; } = null!;
    public DbSet<CompanyEntity> Companies { get; set; } = null!;
    public DbSet<StoreEntity> Stores { get; set; } = null!;
    public DbSet<BonusTransactionEntity> BonusTransactions { get; set; } = null!;
    public DbSet<NotificationEntity> Notifications { get; set; } = null!;
    public DbSet<StoreSellerAssignmentEntity> StoreSellerAssignments { get; set; } = null!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Set schema name
        modelBuilder.HasDefaultSchema("bonus");
        
        // Apply configurations
        modelBuilder.ApplyConfiguration(new UserEntityConfiguration());
        modelBuilder.ApplyConfiguration(new CompanyEntityConfiguration());
        modelBuilder.ApplyConfiguration(new StoreEntityConfiguration());
        modelBuilder.ApplyConfiguration(new BonusTransactionEntityConfiguration());
        modelBuilder.ApplyConfiguration(new NotificationEntityConfiguration());
        modelBuilder.ApplyConfiguration(new StoreSellerAssignmentEntityConfiguration());
    }
}
