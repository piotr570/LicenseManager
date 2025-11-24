using LicenseManager.Licenses.Application;
using LicenseManager.Licenses.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace LicenseManager.Licenses;

public static class LicensesModuleRegistration
{
    public static IServiceCollection AddLicensesModule(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddLicensesApplication();
        services.AddLicensesInfrastructure(connectionString);
        return services;
    }
}

