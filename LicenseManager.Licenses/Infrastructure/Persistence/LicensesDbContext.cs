using LicenseManager.Licenses.Domain.Aggregates;
using LicenseManager.Licenses.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace LicenseManager.Licenses.Infrastructure.Persistence;

public sealed class LicensesDbContext(DbContextOptions<LicensesDbContext> options) : DbContext(options)
{
    public DbSet<License> Licenses { get; set; } = null!;
    public DbSet<Assignment> Assignments { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // License configuration
        modelBuilder.Entity<License>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Key).IsRequired().HasMaxLength(255);
            entity.Property(x => x.Name).IsRequired().HasMaxLength(255);
            entity.Property(x => x.Vendor).IsRequired().HasMaxLength(255);
            entity.Property(x => x.IsActive).IsRequired();
            entity.Property(x => x.UsageCount).IsRequired();
            entity.Property(x => x.IsCancelled).IsRequired();

            // Value object mapping
            entity.OwnsOne(x => x.Terms, terms =>
            {
                terms.Property(x => x.Type).HasColumnName("Terms_Type");
                terms.Property(x => x.Mode).HasColumnName("Terms_Mode");
                terms.Property(x => x.MaxUsers).HasColumnName("Terms_MaxUsers");
                terms.Property(x => x.UsageLimit).HasColumnName("Terms_UsageLimit");
                terms.Property(x => x.ExpirationDate).HasColumnName("Terms_ExpirationDate");
                terms.Property(x => x.IsRenewable).HasColumnName("Terms_IsRenewable");
                terms.Property(x => x.RenewalDate).HasColumnName("Terms_RenewalDate");
            });

            // Relationships
            entity.HasMany(x => x.Assignments)
                .WithOne()
                .HasForeignKey(a => a.LicenseId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(x => x.Key).IsUnique();
        });

        // Assignment configuration
        modelBuilder.Entity<Assignment>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.LicenseId).IsRequired();
            entity.Property(x => x.UserId).IsRequired();
            entity.Property(x => x.AssignedAt).IsRequired();
            entity.Property(x => x.State).IsRequired();

            entity.HasIndex(x => x.LicenseId);
            entity.HasIndex(x => x.UserId);
        });
    }
}

