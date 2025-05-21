using LicenseManager.Domain.Licenses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LicenseManager.Infrastructure.EntityConfigurations;

public class LicenseConfiguration : IEntityTypeConfiguration<License>
{
    public void Configure(EntityTypeBuilder<License> entity)
    {
        entity.HasKey(l => l.Id);
        entity.Ignore(l => l.DomainEvents);
        entity.HasIndex(l => l.Key).IsUnique();

        entity.HasMany(l => l.Assignments)
            .WithOne(la => la.License)
            .HasForeignKey(la => la.LicenseId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(l => l.Terms)
            .WithOne(lt => lt.License)
            .OnDelete(DeleteBehavior.Cascade);
    }
}