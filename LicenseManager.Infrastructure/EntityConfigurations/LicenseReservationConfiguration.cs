using LicenseManager.Domain.Licenses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LicenseManager.Infrastructure.EntityConfigurations;

public class LicenseReservationConfiguration : IEntityTypeConfiguration<LicenseReservation>
{
    public void Configure(EntityTypeBuilder<LicenseReservation> entity)
    {
        entity.ToTable("LicenseReservations");

        entity.HasKey(lr => lr.Id);

        entity.Property(lr => lr.ReservedAt).IsRequired();
        entity.Property(lr => lr.ExpiresAt).IsRequired(false);

        entity.HasIndex(lr => new { lr.LicenseId, lr.UserId }).IsUnique();

        entity.HasOne(lr => lr.License)
            .WithMany(l => l.Reservations)
            .HasForeignKey(lr => lr.LicenseId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(lr => lr.User)
            .WithMany() 
            .HasForeignKey(lr => lr.UserId)
            .OnDelete(DeleteBehavior.Restrict); 
    }
}