using BonusSystem.Infrastructure.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BonusSystem.Infrastructure.DataAccess.EntityFramework.Configurations;

public class StoreEntityConfiguration : IEntityTypeConfiguration<StoreEntity>
{
    public void Configure(EntityTypeBuilder<StoreEntity> builder)
    {
        builder.ToTable("stores");
        
        builder.HasKey(s => s.Id);
        
        builder.Property(s => s.Name)
            .HasMaxLength(200)
            .IsRequired();
        
        builder.Property(s => s.Location)
            .HasMaxLength(100);
        
        builder.Property(s => s.Address)
            .HasMaxLength(255);
        
        builder.Property(s => s.ContactPhone)
            .HasMaxLength(50);
            
        builder.Property(s => s.Category)
            .HasMaxLength(100);
        
        builder.Property(s => s.Status)
            .IsRequired();
        
        builder.Property(s => s.CreatedAt)
            .HasDefaultValueSql("NOW()")
            .IsRequired();
        
        // Relationships
        builder.HasOne(s => s.Company)
            .WithMany()
            .HasForeignKey(s => s.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Indexes
        builder.HasIndex(s => s.Name);
        builder.HasIndex(s => s.CompanyId);
        builder.HasIndex(s => s.Status);
        builder.HasIndex(s => s.Category);
    }
}