using LibraryManagementSystem.Business.Exceptions;
using System.Diagnostics;
using System.Net;
using System.Text.Json;

namespace LibraryBookManagementSystem.API.Middlewares
{
    public class RequestLoggingMiddleware
    {

        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate requestDelegate, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = requestDelegate;
            _logger = logger;
        }


        public async Task Invoke(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                _logger.LogInformation("Incomming Request: {Method} {Path}", context.Request.Method, context.Request.Path);


                await _next(context);

                stopwatch.Stop();

                _logger.LogInformation("Outgoing Response: {Status Code}, in Elapsed time {elapsed time}"
                    , context.Response.StatusCode, stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                _logger.LogError(
                    ex,
                    "Request failed in {ElapsedMs}ms",
                    stopwatch.ElapsedMilliseconds);

                throw;
            }


        }
    }
}
