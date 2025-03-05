using BonusSystem.Infrastructure.DataAccess.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace BonusSystem.Infrastructure.DataAccess.Postgres;

public class BonusSystemDbContext : DbContext
{
    public BonusSystemDbContext(DbContextOptions<BonusSystemDbContext> options) : base(options)
    {
    }

    public DbSet<UserEntity> Users { get; set; } = null!;
    public DbSet<CompanyEntity> Companies { get; set; } = null!;
    public DbSet<StoreEntity> Stores { get; set; } = null!;
    public DbSet<TransactionEntity> Transactions { get; set; } = null!;
    public DbSet<NotificationEntity> Notifications { get; set; } = null!;
    public DbSet<StoreSellerEntity> StoreSellers { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply entity configurations
        ConfigureUser(modelBuilder);
        ConfigureCompany(modelBuilder);
        ConfigureStore(modelBuilder);
        ConfigureTransaction(modelBuilder);
        ConfigureNotification(modelBuilder);
        ConfigureStoreSeller(modelBuilder);
        
        // Set default schema
        modelBuilder.HasDefaultSchema("bonus");
    }
    
    private void ConfigureUser(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserEntity>(entity =>
        {
            entity.ToTable("users");
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.Username);
            entity.HasIndex(e => e.Role);
        });
    }
    
    private void ConfigureCompany(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CompanyEntity>(entity =>
        {
            entity.ToTable("companies");
            entity.HasIndex(e => e.Name);
            entity.HasIndex(e => e.Status);
        });
    }
    
    private void ConfigureStore(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StoreEntity>(entity =>
        {
            entity.ToTable("stores");
            entity.HasIndex(e => e.Name);
            entity.HasIndex(e => e.CompanyId);
            entity.HasIndex(e => e.Status);
        });
    }
    
    private void ConfigureTransaction(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TransactionEntity>(entity =>
        {
            entity.ToTable("transactions");
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.CompanyId);
            entity.HasIndex(e => e.StoreId);
            entity.HasIndex(e => e.Timestamp);
            entity.HasIndex(e => e.Type);
            entity.HasIndex(e => e.Status);
        });
    }
    
    private void ConfigureNotification(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<NotificationEntity>(entity =>
        {
            entity.ToTable("notifications");
            entity.HasIndex(e => e.RecipientId);
            entity.HasIndex(e => e.Type);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => e.IsRead);
        });
    }
    
    private void ConfigureStoreSeller(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StoreSellerEntity>(entity =>
        {
            entity.ToTable("store_sellers");
            entity.HasIndex(e => new { e.StoreId, e.UserId }).IsUnique();
            entity.HasIndex(e => e.UserId);
        });
    }
}