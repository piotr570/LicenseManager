using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using LicenseManager.Licenses.Domain.Repositories;
using LicenseManager.Licenses.Domain.Services;
using LicenseManager.Licenses.Domain.Policies;
using LicenseManager.Licenses.Application.Services;
using LicenseManager.Licenses.Infrastructure.Persistence;
using LicenseManager.Licenses.Infrastructure.BackgroundServices;

namespace LicenseManager.Licenses.Infrastructure;

public static class LicensesInfrastructureRegistration
{
    public static IServiceCollection AddLicensesInfrastructure(
        this IServiceCollection services,
        string connectionString,
        IConfiguration configuration)
    {
        services.AddDbContext<LicensesDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<ILicenseRepository, LicenseRepository>();
        services.AddScoped<IAssignmentRepository, AssignmentRepository>();

        services.AddSingleton<ILicenseAssignmentPolicyFactory, LicenseAssignmentPolicyFactory>();

        services.AddScoped<ILicenseAssignmentCleanupDomainService, LicenseCleanupDomainService>();
        services.AddScoped<ILicenseReservationCleanupDomainService, LicenseCleanupDomainService>();

        services.AddScoped<ILicenseAssignmentCleanupService>(sp =>
        {
            var options = configuration.GetSection("LicenseAssignmentCleanupOptions")
                .Get<LicenseAssignmentCleanupOptions>() ?? new LicenseAssignmentCleanupOptions();
            var domainService = sp.GetRequiredService<ILicenseAssignmentCleanupDomainService>();
            return new LicenseAssignmentCleanupService(domainService, options.DaysInactive);
        });

        services.AddScoped<ILicenseReservationCleanupService>(sp =>
        {
            var options = configuration.GetSection("LicenseReservationCleanupOptions")
                .Get<LicenseReservationCleanupOptions>() ?? new LicenseReservationCleanupOptions();
            var domainService = sp.GetRequiredService<ILicenseReservationCleanupDomainService>();
            return new LicenseReservationCleanupService(domainService, options.DaysReserved);
        });

        services.Configure<LicenseAssignmentCleanupOptions>(
            configuration.GetSection("LicenseAssignmentCleanupOptions"));
        services.Configure<LicenseReservationCleanupOptions>(
            configuration.GetSection("LicenseReservationCleanupOptions"));

        services.AddHostedService<LicenseAssignmentCleanupBackgroundService>();
        services.AddHostedService<LicenseReservationCleanupBackgroundService>();

        return services;
    }
}
