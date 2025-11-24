using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using LicenseManager.Licenses.Domain.Repositories;
using LicenseManager.Licenses.Infrastructure.Persistence;

namespace LicenseManager.Licenses.Infrastructure;

public static class LicensesInfrastructureRegistration
{
    public static IServiceCollection AddLicensesInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<LicensesDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<ILicenseRepository, LicenseRepository>();
        services.AddScoped<IAssignmentRepository, AssignmentRepository>();

        return services;
    }
}

