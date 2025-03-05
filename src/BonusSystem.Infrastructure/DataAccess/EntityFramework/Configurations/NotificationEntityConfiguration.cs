using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BonusSystem.Infrastructure.DataAccess.EntityFramework.Configurations;

public class NotificationEntityConfiguration : IEntityTypeConfiguration<NotificationEntityConfiguration>
{
    public void Configure(EntityTypeBuilder<NotificationEntityConfiguration> builder)
    {
        throw new NotImplementedException();
    }
}