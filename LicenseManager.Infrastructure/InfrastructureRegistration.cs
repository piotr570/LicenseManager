using LicenseManager.Application.Abstraction;
using LicenseManager.Application.HostedServices;
using LicenseManager.Application.HostedServices.Interfaces;
using LicenseManager.Domain.Abstractions;
using LicenseManager.Infrastructure.BackgroundServices;
using LicenseManager.Infrastructure.Configuration;
using LicenseManager.Infrastructure.Persistence;
using LicenseManager.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LicenseManager.Infrastructure;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DatabaseConfiguration>(
            configuration.GetSection("DatabaseConfiguration"));
        
        services.Configure<LicenseAssignmentCleanupSettings>(
            configuration.GetSection("LicenseAssignmentCleanupSettings"));
        
        services.Configure<LicenseReservationCleanupSettings>(
            configuration.GetSection("LicenseReservationCleanupSettings"));
        
        services.AddDbContextPool<LicenseManagerDbContext>((sp, opt) =>
        {
            var cfg = sp.GetRequiredService<IOptions<DatabaseConfiguration>>().Value;
            opt.UseNpgsql(cfg.ConnectionString);
        });
            
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        services.AddScoped<IUnitOfWork, UnitOfWork<LicenseManagerDbContext>>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>)); 
        services.AddScoped<ILicenseReservationCleanupService, LicenseReservationCleanupService>();
        services.AddScoped<ILicenseAssignmentCleanupService, LicenseAssignmentCleanupService>();
        services.AddHostedService<LicenseReservationCleanupBackgroundService>();
        services.AddHostedService<LicenseAssignmentCleanupBackgroundService>();

        return services;
    }
}
