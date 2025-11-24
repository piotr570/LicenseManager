using System.Reflection;
using LicenseManager.Application.Abstraction;
using LicenseManager.Application.HostedServices;
using LicenseManager.Domain.Assignments;
using Microsoft.Extensions.DependencyInjection;

namespace LicenseManager.Application;

public static class ApplicationRegistration
{
    public static readonly Assembly ApplicationAssembly = typeof(ApplicationRegistration).Assembly;
    
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(ApplicationAssembly));
        services.AddSingleton<ILicenseAssignmentPolicyFactory, LicenseAssignmentPolicyFactory>();

        services.AddScoped<ILicenseAssignmentCleanupService, LicenseAssignmentCleanupService>();
        services.AddScoped<ILicenseReservationCleanupService, LicenseReservationCleanupService>();

        return services;
    }
}