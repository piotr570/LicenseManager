using LicenseManager.Domain.Licenses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LicenseManager.Infrastructure.Configurations;

public class LicenseConfiguration : IEntityTypeConfiguration<License>
{
    public void Configure(EntityTypeBuilder<License> entity)
    {
        entity.HasKey(x => x.Id);
        entity.HasIndex(x => x.Key).IsUnique();
        entity.Property(x => x.Name).IsRequired();
        entity.Property(x => x.Vendor).IsRequired();
        entity.Property(x => x.IsActive).IsRequired();
        entity.Property(x => x.UsageCount).IsRequired();
        entity.Property(x => x.IsPaymentValid).IsRequired(false);
        entity.Property(x => x.IsCancelled).IsRequired();
        entity.Property(x => x.Department).HasConversion<string>().IsRequired(false);
        
        entity.OwnsOne(x => x.Terms);
        
        // entity.HasMany(x => x.Assignments)
        //     .WithOne()
        //     .HasForeignKey(a => a.LicenseId)
        //     .OnDelete(DeleteBehavior.Cascade);

        // // Explicitly tell EF to use field access for the Assignments navigation (backing field _assignments)
        // entity.Navigation(x => x.Assignments)
        //     .UsePropertyAccessMode(PropertyAccessMode.Field);

        entity.Ignore(x => x.ReservationIds);
        entity.Ignore(x => x.DomainEvents);
    }
}