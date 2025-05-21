using LicenseManager.Domain.Licenses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LicenseManager.Infrastructure.EntityConfigurations;

public class LicenseTermsConfiguration : IEntityTypeConfiguration<LicenseTerms>
{
    public void Configure(EntityTypeBuilder<LicenseTerms> entity)
    {
        entity.HasKey(lt => lt.Id);

        entity.HasOne(lt => lt.License)
            .WithOne(l => l.Terms)
            .OnDelete(DeleteBehavior.Cascade);
    }
}