using LicenseManager.Domain.Assignments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LicenseManager.Infrastructure.EntityConfigurations;

public class LicenseAssignmentConfiguration : IEntityTypeConfiguration<Assignment>
{
    public void Configure(EntityTypeBuilder<Assignment> entity)
    {
        entity.HasKey(la => la.Id);

        entity.HasOne(la => la.License)
            .WithMany(l => l.Assignments)
            .HasForeignKey(la => la.LicenseId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(la => la.User)
            .WithMany(u => u.LicenseAssignments)
            .HasForeignKey(la => la.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}