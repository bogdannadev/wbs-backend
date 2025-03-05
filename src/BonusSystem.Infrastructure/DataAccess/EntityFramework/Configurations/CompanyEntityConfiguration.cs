using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BonusSystem.Infrastructure.DataAccess.EntityFramework.Configurations;

public class CompanyEntityConfiguration : IEntityTypeConfiguration<CompanyEntityConfiguration>
{
    public void Configure(EntityTypeBuilder<CompanyEntityConfiguration> builder)
    {
        throw new NotImplementedException();
    }
}