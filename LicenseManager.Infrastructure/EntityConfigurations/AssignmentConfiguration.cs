using LicenseManager.Domain.Assignments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LicenseManager.Infrastructure.EntityConfigurations;

public class AssignmentConfiguration : IEntityTypeConfiguration<Assignment>
{
    public void Configure(EntityTypeBuilder<Assignment> entity)
    {
        entity.ToTable("Assignments");
        entity.HasKey(x => x.Id);
        entity.Property(x => x.LicenseId).IsRequired();
        entity.Property(x => x.UserId).IsRequired();
        entity.Property(x => x.AssignedAt).IsRequired();
        entity.Property(x => x.LastInvokedAt).IsRequired(false);
        entity.Property(x => x.State).HasConversion<string>().IsRequired();
        entity.HasIndex(x => new { x.LicenseId, x.UserId }).IsUnique();
    }
}