using System.Reflection;
using LicenseManager.Domain.Assignments;
using LicenseManager.Domain.Licenses;
using LicenseManager.Domain.Users;
using LicenseManager.SharedKernel.Events;
using Microsoft.EntityFrameworkCore;

namespace LicenseManager.Infrastructure;

public class LicenseManagerDbContext(DbContextOptions<LicenseManagerDbContext> options)
    : DbContext(options)
{
    public DbSet<License> Licenses { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Assignment> LicenseAssignments { get; set; }
    public DbSet<LicenseReservation> LicenseReservations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Ignore<DomainEvent>();
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}