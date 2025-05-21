using System.Reflection;
using LicenseManager.Domain.Licenses.Factories.Assignment;
using Microsoft.Extensions.DependencyInjection;

namespace LicenseManager.Application;

public static class ApplicationRegistration
{
    public static readonly Assembly ApplicationAssembly = typeof(ApplicationRegistration).Assembly;
    
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(ApplicationAssembly));
        services.AddSingleton<ILicenseAssignmentPolicyFactory, LicenseAssignmentPolicyFactory>();

        return services;
    }
}