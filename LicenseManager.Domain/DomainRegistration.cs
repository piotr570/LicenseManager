using LicenseManager.Domain.Assignments;
using LicenseManager.Domain.Licenses.Abstractions;
using LicenseManager.Domain.Licenses.Factories.Creation;
using LicenseManager.Domain.Licenses.Policies;
using LicenseManager.Domain.Licenses.Rules;
using LicenseManager.Domain.Licenses.Services;
using LicenseManager.Domain.Reservations.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LicenseManager.Domain;

public static class DomainRegistration
{
    public static IServiceCollection AddDomainLayer(this IServiceCollection services)
    {
        services.AddScoped<ILicenseAssignmentCleanupDomainService, LicenseAssignmentCleanupDomainService>();
        services.AddScoped<IReservationDomainService, ReservationDomainService>();
        services.AddSingleton<SingleLicenseAssignmentPolicy>();
        services.AddSingleton<TeamLicenseAssignmentPolicy>();
        services.AddSingleton<ServerLicenseAssignmentPolicy>();
        services.AddSingleton<ILicenseAssignmentPolicyFactory, LicenseAssignmentPolicyFactory>();

        return services;
    }
}