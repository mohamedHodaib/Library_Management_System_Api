using LibraryManagementSystem.Business.Exceptions;
using System.Net;
using System.Text.Json;

namespace LibraryBookManagementSystem.API.Middlewares
{
    public class RequestLoggingHandler
    {

        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingHandler> _logger;

        public RequestLoggingHandler(RequestDelegate requestDelegate, ILogger<RequestLoggingHandler> logger)
        {
            _next = requestDelegate;
            _logger = logger;
        }


        public async Task Invoke(HttpContext context)
        {
           _logger.LogInformation("Incomming Request: {Method} {Path}",context.Request.Method,context.Request.Path);

            await _next(context);

            _logger.LogInformation("Outgoing Response: {Status Code}",context.Response.StatusCode);
        }
    }
}
