using LicenseManager.Domain.Reservations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LicenseManager.Infrastructure.Configurations;

public class LicenseReservationConfiguration : IEntityTypeConfiguration<LicenseReservation>
{
    public void Configure(EntityTypeBuilder<LicenseReservation> entity)
    {
        entity.ToTable("LicenseReservations");

        entity.HasKey(lr => lr.Id);

        entity.Property(lr => lr.ReservedAt).IsRequired();
        entity.Property(lr => lr.ExpirationDate).IsRequired(false);
        entity.Property(lr => lr.LicenseId).IsRequired();
        entity.Property(lr => lr.UserId).IsRequired();

        entity.HasIndex(lr => new { lr.LicenseId, lr.UserId }).IsUnique();
    }
}