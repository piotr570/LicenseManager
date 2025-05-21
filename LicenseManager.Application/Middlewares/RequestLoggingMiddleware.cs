using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace LicenseManager.Application.Middlewares;

public class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        var request = context.Request;
        logger.LogInformation("HTTP Request: {method} {path}", request.Method, request.Path);

        await next(context); 

        var response = context.Response;
        stopwatch.Stop();
        logger.LogInformation("HTTP Response: {statusCode} - {elapsed}ms", response.StatusCode, stopwatch.ElapsedMilliseconds);
    }
}