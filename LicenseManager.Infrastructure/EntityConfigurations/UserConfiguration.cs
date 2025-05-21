using LicenseManager.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LicenseManager.Infrastructure.EntityConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> entity)
    {
        entity.HasKey(u => u.Id);

        entity.Property(u => u.Department)
            .HasConversion<string>();

        entity.HasMany(u => u.LicenseAssignments)
            .WithOne(la => la.User)
            .HasForeignKey(la => la.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasMany(u => u.LicenseReservations)
            .WithOne(lr => lr.User)
            .HasForeignKey(lr => lr.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}