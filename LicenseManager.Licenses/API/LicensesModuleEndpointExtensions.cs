using Microsoft.AspNetCore.Routing;

namespace LicenseManager.Licenses.API;

public static class LicensesModuleEndpointExtensions
{
    public static IEndpointRouteBuilder MapLicensesModule(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapLicensesModuleEndpoints();
        return endpoints;
    }
}