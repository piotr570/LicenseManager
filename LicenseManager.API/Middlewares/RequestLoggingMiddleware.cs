using LicenseManager.SharedKernel.Common;

namespace LicenseManager.Middlewares;

public sealed class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        logger.LogInformation(
            "HTTP Request: {Method} {Path} - Started at {Timestamp}",
            context.Request.Method,
            context.Request.Path,
            SystemClock.Now);

        await next(context);

        logger.LogInformation(
            "HTTP Response: {Method} {Path} - Status {StatusCode} - Completed at {Timestamp}",
            context.Request.Method,
            context.Request.Path,
            context.Response.StatusCode,
            SystemClock.Now);
    }
}