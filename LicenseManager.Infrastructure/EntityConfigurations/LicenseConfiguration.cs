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
        entity.Property(l => l.Name).IsRequired();
        entity.Property(l => l.Vendor).IsRequired();
        entity.Property(l => l.IsActive).IsRequired();
        entity.Property(l => l.UsageCount).IsRequired();
        entity.Property(l => l.IsPaymentValid).IsRequired(false);
        entity.Property(l => l.IsCancelled).IsRequired();
        entity.Property(l => l.Department).HasConversion<string>().IsRequired(false);
        
        // If LicenseTerms is a value object, configure as owned entity:
        entity.OwnsOne(l => l.Terms);
        
        entity.Ignore(l => l.AssignmentIds);
        entity.Ignore(l => l.ReservationIds);
    }
}