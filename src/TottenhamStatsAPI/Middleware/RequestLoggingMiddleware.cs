using System.Diagnostics;

namespace TottenhamStatsAPI.Middleware;

public class RequestLoggingMiddleware
{
    private readonly ILogger<RequestLoggingMiddleware> _logger;
    private readonly RequestDelegate _next;

    public RequestLoggingMiddleware(
        RequestDelegate next,
        ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            stopwatch.Stop();

            _logger.LogError(
                exception,
                "HTTP {Method} {Path}{QueryString} failed in {ElapsedMilliseconds} ms",
                context.Request.Method,
                context.Request.Path,
                context.Request.QueryString,
                stopwatch.ElapsedMilliseconds);

            throw;
        }
        finally
        {
            if (stopwatch.IsRunning)
            {
                stopwatch.Stop();
                LogRequest(context, stopwatch.ElapsedMilliseconds);
            }
        }
    }

    private void LogRequest(HttpContext context, long elapsedMilliseconds)
    {
        var statusCode = context.Response.StatusCode;

        if (statusCode >= StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(
                "HTTP {Method} {Path}{QueryString} responded {StatusCode} in {ElapsedMilliseconds} ms",
                context.Request.Method,
                context.Request.Path,
                context.Request.QueryString,
                statusCode,
                elapsedMilliseconds);

            return;
        }

        if (statusCode >= StatusCodes.Status400BadRequest)
        {
            _logger.LogWarning(
                "HTTP {Method} {Path}{QueryString} responded {StatusCode} in {ElapsedMilliseconds} ms",
                context.Request.Method,
                context.Request.Path,
                context.Request.QueryString,
                statusCode,
                elapsedMilliseconds);

            return;
        }

        _logger.LogInformation(
            "HTTP {Method} {Path}{QueryString} responded {StatusCode} in {ElapsedMilliseconds} ms",
            context.Request.Method,
            context.Request.Path,
            context.Request.QueryString,
            statusCode,
            elapsedMilliseconds);
    }
}