using BonusSystem.Infrastructure.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BonusSystem.Infrastructure.DataAccess.EntityFramework.Configurations;

public class FiatTransactionEntityConfiguration : IEntityTypeConfiguration<FiatTransactionEntity>
{
    public void Configure(EntityTypeBuilder<FiatTransactionEntity> builder)
    {
        builder.ToTable("transactions_fiat");

        builder.HasKey(f => f.Id);

        builder.Property(f => f.BuyerId)
            .IsRequired();

        builder.Property(f => f.PaymentBody)
            .IsRequired();

        builder.Property(f => f.Description)
            .HasMaxLength(500);

        builder.Property(f => f.TransactionDate)
            .IsRequired();

        builder.Property(f => f.Status)
            .IsRequired();

        // Configure navigation properties if needed
        builder.HasOne(f => f.User)
            .WithMany()
            .HasForeignKey("UserId")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(f => f.Company)
            .WithMany()
            .HasForeignKey("CompanyId")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(f => f.Store)
            .WithMany()
            .HasForeignKey("StoreId")
            .OnDelete(DeleteBehavior.Restrict);
    }
}