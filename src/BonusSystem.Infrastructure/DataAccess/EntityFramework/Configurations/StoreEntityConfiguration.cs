using BonusSystem.Infrastructure.DataAccess.Postgres.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BonusSystem.Infrastructure.DataAccess.EntityFramework.Configurations;

public class StoreEntityConfiguration : IEntityTypeConfiguration<StoreEntity>
{
    public void Configure(EntityTypeBuilder<StoreEntity> builder)
    {
        throw new NotImplementedException();
    }
}