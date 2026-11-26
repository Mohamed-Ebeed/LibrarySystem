using System.Diagnostics;

namespace LibrarySystem.Middleware
{
    public sealed class RequestTimingMiddleware(RequestDelegate next, ILogger<RequestTimingMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                await next(context);
            }
            finally
            {
                stopwatch.Stop();
                logger.LogInformation("{Method} {Path} => {StatusCode} in {Elapsed} ms",
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    stopwatch.ElapsedMilliseconds);
            }
        }
    }
}