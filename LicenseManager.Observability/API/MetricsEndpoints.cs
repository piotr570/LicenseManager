using LicenseManager.Observability.Metrics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LicenseManager.Observability.API;

internal static class MetricsEndpoints
{
    internal static IEndpointRouteBuilder MapMetricsEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var metricsGroup = endpoints.MapGroup("/api/metrics")
            .WithTags("Metrics");

        // GET /api/metrics/cleanup/statistics
        metricsGroup.MapGet("/cleanup/statistics", (CleanupMetricsCollector metricsCollector) =>
        {
            var statistics = metricsCollector.GetAllStatistics();
            return Results.Ok(statistics);
        })
        .WithName("GetAllCleanupStatistics")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Get all cleanup service statistics";
            operation.Description = "Returns aggregated statistics for all cleanup services";
            return operation;
        })
        .Produces<Dictionary<string, CleanupServiceStatistics>>(StatusCodes.Status200OK);

        // GET /api/metrics/cleanup/statistics/{serviceName}
        metricsGroup.MapGet("/cleanup/statistics/{serviceName}", 
            (string serviceName, CleanupMetricsCollector metricsCollector) =>
        {
            var statistics = metricsCollector.GetStatistics(serviceName);
            
            if (statistics.TotalRuns == 0)
            {
                return Results.NotFound(new { message = $"No metrics found for service: {serviceName}" });
            }

            return Results.Ok(statistics);
        })
        .WithName("GetServiceStatistics")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Get statistics for a specific cleanup service";
            operation.Description = "Returns detailed statistics for the specified cleanup service";
            return operation;
        })
        .Produces<CleanupServiceStatistics>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        // GET /api/metrics/cleanup/history?limit=50
        metricsGroup.MapGet("/cleanup/history", 
            (CleanupMetricsCollector metricsCollector, int limit = 50) =>
        {
            var history = metricsCollector.GetAllMetrics()
                .OrderByDescending(x => x.StartTime)
                .Take(Math.Min(limit, 100))
                .ToList();

            return Results.Ok(history);
        })
        .WithName("GetCleanupHistory")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Get recent execution history for all services";
            operation.Description = "Returns the most recent cleanup executions across all services (max 100)";
            return operation;
        })
        .Produces<List<CleanupServiceMetrics>>(StatusCodes.Status200OK);

        // GET /api/metrics/cleanup/history/{serviceName}?limit=20
        metricsGroup.MapGet("/cleanup/history/{serviceName}", 
            (string serviceName, CleanupMetricsCollector metricsCollector, int limit = 20) =>
        {
            var history = metricsCollector.GetMetricsByService(serviceName)
                .OrderByDescending(x => x.StartTime)
                .Take(Math.Min(limit, 100))
                .ToList();

            return Results.Ok(history);
        })
        .WithName("GetServiceHistory")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Get recent execution history for a specific service";
            operation.Description = "Returns the most recent cleanup executions for the specified service (max 100)";
            return operation;
        })
        .Produces<List<CleanupServiceMetrics>>(StatusCodes.Status200OK);

        // GET /api/metrics/cleanup/latest/{serviceName}
        metricsGroup.MapGet("/cleanup/latest/{serviceName}", 
            (string serviceName, CleanupMetricsCollector metricsCollector) =>
        {
            var metrics = metricsCollector.GetLatestMetrics(serviceName);
            
            if (metrics == null)
            {
                return Results.NotFound(new { message = $"No metrics found for service: {serviceName}" });
            }

            return Results.Ok(metrics);
        })
        .WithName("GetLatestMetrics")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Get the latest execution metrics for a specific service";
            operation.Description = "Returns the most recent execution metrics for the specified cleanup service";
            return operation;
        })
        .Produces<CleanupServiceMetrics>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        return endpoints;
    }
}
