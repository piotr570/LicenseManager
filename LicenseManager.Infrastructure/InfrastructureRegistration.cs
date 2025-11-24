using LicenseManager.Application.Abstraction;
using LicenseManager.Infrastructure.BackgroundServices;
using LicenseManager.Infrastructure.Configuration;
using LicenseManager.Infrastructure.Options;
using LicenseManager.Infrastructure.Persistence;
using LicenseManager.Infrastructure.Services;
using LicenseManager.SharedKernel.Abstractions;
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
        
        services.Configure<LicenseAssignmentCleanupOptions>(
            configuration.GetSection("LicenseAssignmentCleanupOptions"));
        
        services.Configure<LicenseReservationCleanupOptions>(
            configuration.GetSection("LicenseReservationCleanupOptions"));
        
        services.AddDbContextPool<LicenseManagerDbContext>((sp, opt) =>
        {
            var cfg = sp.GetRequiredService<IOptions<DatabaseConfiguration>>().Value;
            opt.UseNpgsql(cfg.ConnectionString);
        });

        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        services.AddScoped<IReadDbContext, ReadDbContext>();
        services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork<LicenseManagerDbContext>));
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        
        services.AddHostedService<LicenseAssignmentCleanupBackgroundService>();

        return services;
    }
}
