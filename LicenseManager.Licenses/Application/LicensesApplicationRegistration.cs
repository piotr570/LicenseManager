using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using MediatR;

namespace LicenseManager.Licenses.Application;

public static class LicensesApplicationRegistration
{
    private static readonly Assembly ApplicationAssembly = typeof(LicensesApplicationRegistration).Assembly;

    public static IServiceCollection AddLicensesApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(ApplicationAssembly));
        return services;
    }
}
