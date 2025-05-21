using LicenseManager.Domain.Licenses.Abstractions;
using LicenseManager.Domain.Licenses.Factories.Creation;
using LicenseManager.Domain.Licenses.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LicenseManager.Domain;

public static class DomainRegistration
{
    public static IServiceCollection AddDomainLayer(this IServiceCollection services)
    {
        services.AddScoped<ILicenseAssignmentCleanupDomainService, LicenseAssignmentCleanupDomainService>();
        services.AddScoped<ILicenseBusinessRuleProvider, LicenseBusinessRuleProvider>();
        services.AddScoped<ILicenseFactory, LicenseFactory>();

        return services;
    }
}