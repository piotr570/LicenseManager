using LicenseManager.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LicenseManager.Infrastructure.EntityConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> entity)
    {
        entity.HasKey(u => u.Id);

        entity.Property(u => u.Email)
            .IsRequired();

        entity.Property(u => u.Name)
            .IsRequired();

        entity.Property(u => u.Department)
            .HasConversion<string>();
    }
}