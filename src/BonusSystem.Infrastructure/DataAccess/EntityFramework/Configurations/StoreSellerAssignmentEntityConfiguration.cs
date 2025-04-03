using BonusSystem.Infrastructure.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BonusSystem.Infrastructure.DataAccess.EntityFramework.Configurations;

public class StoreSellerAssignmentEntityConfiguration : IEntityTypeConfiguration<StoreSellerAssignmentEntity>
{
    
    public void Configure(EntityTypeBuilder<StoreSellerAssignmentEntity> builder)
    {
        builder.ToTable("store_sellers");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.AssignedAt)
            .HasDefaultValueSql("NOW()")
            .IsRequired();
        
        // Relationships
        builder.HasOne(s => s.Store)
            .WithMany(s => s.SellerAssignments)
            .HasForeignKey(s => s.StoreId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.User)
            .WithMany(u => u.StoreAssignments)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Indexes
        builder.HasIndex(s => new { s.StoreId, s.UserId }).IsUnique();
        builder.HasIndex(s => s.UserId);
    }
}