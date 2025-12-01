using LicenseManager.Licenses.Application;
using LicenseManager.Licenses.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LicenseManager.Licenses;

public static class LicensesModuleRegistration
{
    private static IServiceCollection AddLicensesModule(
        this IServiceCollection services,
        string connectionString,
        IConfiguration configuration)
    {
        services.AddLicensesApplication();
        services.AddLicensesInfrastructure(connectionString, configuration);
        return services;
    }

    public static IServiceCollection AddLicensesModule(
        this IServiceCollection services,
        string connectionString)
    {
        var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
        return services.AddLicensesModule(connectionString, configuration);
    }
}
