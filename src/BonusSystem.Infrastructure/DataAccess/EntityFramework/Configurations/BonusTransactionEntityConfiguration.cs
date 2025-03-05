using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BonusSystem.Infrastructure.DataAccess.EntityFramework.Configurations;

public class BonusTransactionEntityConfiguration : IEntityTypeConfiguration<BonusTransactionEntityConfiguration>
{
    public void Configure(EntityTypeBuilder<BonusTransactionEntityConfiguration> builder)
    {
        throw new NotImplementedException();
    }
}