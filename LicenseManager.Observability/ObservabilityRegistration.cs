using LicenseManager.Observability.API;
using LicenseManager.Observability.HealthChecks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace LicenseManager.Observability;

public static class ObservabilityModuleRegistration
{
    public static IServiceCollection AddObservabilityModule(this IServiceCollection services)
    {
        services.AddSingleton<CleanupMetricsCollector>();

        services.AddSingleton<BackgroundServiceHealthCheck>();

        services.AddHealthChecks()
            .AddCheck<BackgroundServiceHealthCheck>("background_services");

        return services;
    }

    public static IEndpointRouteBuilder MapObservabilityModuleEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapMetricsEndpoints();
        endpoints.MapHealthChecks("/health");
        endpoints.MapHealthChecks("/health/ready");
        endpoints.MapHealthChecks("/health/live");
        return endpoints;
    }
}