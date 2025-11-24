using LicenseManager.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LicenseManager.Infrastructure.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> entity)
    {
        entity.HasKey(x => x.Id);

        entity.Property(x => x.Email)
            .IsRequired();

        entity.Property(x => x.Name)
            .IsRequired();

        entity.Property(x => x.Department)
            .HasConversion<string>();
    }
}