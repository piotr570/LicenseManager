namespace LicenseManager.Middlewares;

public sealed class ExceptionHandlingMiddleware(RequestDelegate next, 
    ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled exception has occurred while executing the request.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new ErrorResponse
        {
            Message = exception.Message,
            Type = exception.GetType().Name,
        };

        context.Response.StatusCode = exception switch
        {
            ArgumentNullException or ArgumentException => StatusCodes.Status400BadRequest,
            InvalidOperationException => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };

        return context.Response.WriteAsJsonAsync(response);
    }
}

public class ErrorResponse
{
    public string Message { get; set; } = null!;
    public string Type { get; set; } = null!;
}